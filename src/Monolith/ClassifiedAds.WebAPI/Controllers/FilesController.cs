using ClassifiedAds.Application;
using ClassifiedAds.Application.AuditLogEntries.DTOs;
using ClassifiedAds.Application.AuditLogEntries.Queries;
using ClassifiedAds.Application.FileEntries.Queries;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Infrastructure.Storages;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Infrastructure.AI;
using ClassifiedAds.WebAPI.Authorization;
using ClassifiedAds.WebAPI.ConfigurationOptions;
using ClassifiedAds.WebAPI.Hubs;
using ClassifiedAds.WebAPI.Models.Files;
using CryptographyHelper;
using CryptographyHelper.SymmetricAlgorithms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.WebAPI.Controllers;

[Authorize]
[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class FilesController : Controller
{
    private readonly Dispatcher _dispatcher;
    private readonly AppSettings _options;
    private readonly IFileStorageManager _fileManager;
    private readonly IMemoryCache _memoryCache;
    private readonly IHubContext<NotificationHub> _notificationHubContext;
    private readonly IStringLocalizer _stringLocalizer;
    private readonly IAuthorizationService _authorizationService;
    private readonly IRepository<FileEntryText, Guid> _fileEntryTextRepository;
    private readonly IRepository<FileEntryEmbedding, Guid> _fileEntryEmbeddingRepository;
    private readonly EmbeddingService _embeddingService;

    public FilesController(Dispatcher dispatcher,
        IOptions<AppSettings> options,
        IFileStorageManager fileManager,
        IMemoryCache memoryCache,
        IHubContext<NotificationHub> notificationHubContext,
        IStringLocalizer stringLocalizer,
        IAuthorizationService authorizationService,
        IRepository<FileEntryText, Guid> fileEntryTextRepository,
        IRepository<FileEntryEmbedding, Guid> fileEntryEmbeddingRepository,
        EmbeddingService embeddingService)
    {
        _dispatcher = dispatcher;
        _options = options.Value;
        _fileManager = fileManager;
        _memoryCache = memoryCache;
        _notificationHubContext = notificationHubContext;
        _stringLocalizer = stringLocalizer;
        _authorizationService = authorizationService;
        _fileEntryTextRepository = fileEntryTextRepository;
        _fileEntryEmbeddingRepository = fileEntryEmbeddingRepository;
        _embeddingService = embeddingService;
    }

    [Authorize(Permissions.GetFiles)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FileEntryModel>>> Get()
    {
        await _notificationHubContext.Clients.All.SendAsync("ReceiveMessage", $"{_stringLocalizer["Getting files ..."]}");
        var fileEntries = await _dispatcher.DispatchAsync(new GetFileEntriesQuery());
        return Ok(fileEntries.ToModels());
    }

    [Authorize(Permissions.GetFiles)]
    [HttpGet("vectorsearch")]
    public async Task<ActionResult<IEnumerable<FileEntryVectorSearchResultModel>>> VectorSearch(string searchText)
    {
        var embeddingRs = await _embeddingService.GenerateAsync(searchText);
        var embedding = new SqlVector<float>(embeddingRs.Vector);

        var chunks = _fileEntryEmbeddingRepository.GetQueryableSet()
                .Where(x => !x.FileEntry.Deleted)
                .OrderBy(x => EF.Functions.VectorDistance("cosine", x.Embedding, embedding))
                .Take(5)
                .Select(x => new
                {
                    x.FileEntry,
                    x.ChunkName,
                    x.ChunkLocation,
                    x.ShortText,
                    SimilarityScore = EF.Functions.VectorDistance("cosine", x.Embedding, embedding)
                }).ToList();

        var results = new List<FileEntryVectorSearchResultModel>();

        foreach (var chunk in chunks)
        {
            var result = new FileEntryVectorSearchResultModel
            {
                FileEntryId = chunk.FileEntry.Id,
                FileEntryName = chunk.FileEntry.Name,
                FileName = chunk.FileEntry.FileName,
                ChunkName = chunk.ChunkName,
                SimilarityScore = chunk.SimilarityScore
            };

            var fileExtension = Path.GetExtension(chunk.FileEntry.FileName);

            if (fileExtension == ".jpg" || fileExtension == ".png")
            {
                var content = await GetBytesAsync(chunk.FileEntry);
                result.ChunkData = $"data:image/{fileExtension.TrimStart('.')};base64,{Convert.ToBase64String(content)}";
            }
            else
            {
                result.ChunkData = chunk.ShortText;
            }

            result.FileExtension = fileExtension;

            results.Add(result);
        }

        return Ok(results);
    }

    [Authorize(Permissions.UploadFile)]
    [HttpPost]
    public async Task<ActionResult<FileEntryModel>> Upload([FromForm] UploadFileModel model)
    {
        var fileEntry = new FileEntry
        {
            Name = model.Name,
            Description = model.Description,
            Size = model.FormFile.Length,
            UploadedTime = DateTime.Now,
            FileName = model.FormFile.FileName,
            Encrypted = model.Encrypted,
        };

        await _dispatcher.DispatchAsync(new AddOrUpdateEntityCommand<FileEntry>(fileEntry));

        fileEntry.FileLocation = DateTime.Now.ToString("yyyy/MM/dd/") + fileEntry.Id;

        await _dispatcher.DispatchAsync(new AddOrUpdateEntityCommand<FileEntry>(fileEntry));

        if (model.Encrypted)
        {
            var key = SymmetricCrypto.GenerateKey(32);
            var iv = SymmetricCrypto.GenerateKey(16);
            using (var inputStream = model.FormFile.OpenReadStream())
            using (var encryptedStream = new MemoryStream(inputStream
                    .UseAES(key)
                    .WithCipher(CipherMode.CBC)
                    .WithIV(iv)
                    .WithPadding(PaddingMode.PKCS7)
                    .Encrypt()))
            {
                await _fileManager.CreateAsync(fileEntry, encryptedStream);
            }

            var masterEncryptionKey = _options.Storage.MasterEncryptionKey;
            var encryptedKey = key
                .UseAES(masterEncryptionKey.FromBase64String())
                .WithCipher(CipherMode.CBC)
                .WithIV(iv)
                .WithPadding(PaddingMode.PKCS7)
                .Encrypt();

            fileEntry.EncryptionKey = encryptedKey.ToBase64String();
            fileEntry.EncryptionIV = iv.ToBase64String();
        }
        else
        {
            using var stream = model.FormFile.OpenReadStream();
            await _fileManager.CreateAsync(fileEntry, stream);
        }

        await _dispatcher.DispatchAsync(new AddOrUpdateEntityCommand<FileEntry>(fileEntry));

        return Ok(fileEntry.ToModel());
    }

    [Authorize(Permissions.GetFile)]
    [HttpGet("{id}")]
    public async Task<ActionResult<IEnumerable<FileEntryModel>>> Get(Guid id)
    {
        var fileEntry = await _dispatcher.DispatchAsync(new GetEntityByIdQuery<FileEntry> { Id = id });

        if (fileEntry == null || fileEntry.Deleted)
        {
            // return NotFound();
            return Ok(null);
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, fileEntry, Operations.Read);
        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }

        var model = fileEntry.ToModel();

        model.FileEntryText = _fileEntryTextRepository.GetQueryableSet()
            .Where(x => x.FileEntryId == fileEntry.Id)
            .Select(x => new FileEntryTextModel
            {
                TextLocation = x.TextLocation
            }).FirstOrDefault();

        model.FileEntryEmbeddings = _fileEntryEmbeddingRepository.GetQueryableSet()
            .Where(x => x.FileEntryId == fileEntry.Id)
            .OrderBy(x => x.CreatedDateTime)
            .Select(x => new
            {
                x.ChunkName,
                x.ChunkLocation,
                x.ShortText,
                x.Embedding,
                x.TokenDetails,
                x.CreatedDateTime,
                x.UpdatedDateTime,
            })
            .AsEnumerable()
            .Select(x => new FileEntryEmbeddingModel
            {
                ChunkName = x.ChunkName,
                ChunkLocation = x.ChunkLocation,
                ShortText = x.ShortText,
                Embedding = JsonSerializer.Serialize(x.Embedding.Memory),
                TokenDetails = x.TokenDetails,
                CreatedDateTime = x.CreatedDateTime,
                UpdatedDateTime = x.UpdatedDateTime,
            })
            .ToList();

        return Ok(model);
    }

    [Authorize(Permissions.DownloadFile)]
    [HttpGet("{id}/download")]
    public async Task<IActionResult> Download(Guid id)
    {
        var fileEntry = await _dispatcher.DispatchAsync(new GetEntityByIdQuery<FileEntry> { Id = id });

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, fileEntry, Operations.Read);
        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }

        var content = await GetBytesAsync(fileEntry);

        return File(content, MediaTypeNames.Application.Octet, WebUtility.HtmlEncode(fileEntry.FileName));
    }

    private async Task<byte[]> GetBytesAsync(FileEntry fileEntry, CancellationToken cancellationToken = default)
    {
        var content = await _fileManager.ReadAsync(fileEntry, cancellationToken);

        if (fileEntry.Encrypted)
        {
            var masterEncryptionKey = _options.Storage.MasterEncryptionKey;
            var encryptionKey = fileEntry.EncryptionKey.FromBase64String()
                      .UseAES(masterEncryptionKey.FromBase64String())
                      .WithCipher(CipherMode.CBC)
                      .WithIV(fileEntry.EncryptionIV.FromBase64String())
                      .WithPadding(PaddingMode.PKCS7)
                      .Decrypt();

            content = fileEntry.FileLocation != "Fake.txt"
                ? content
                .UseAES(encryptionKey)
                .WithCipher(CipherMode.CBC)
                .WithIV(fileEntry.EncryptionIV.FromBase64String())
                .WithPadding(PaddingMode.PKCS7)
                .Decrypt()
                : content;
        }

        return content;
    }

    [Authorize(Permissions.DownloadFile)]
    [HttpGet("{id}/downloadtext")]
    public async Task<IActionResult> DownloadText(Guid id)
    {
        var fileEntry = await _dispatcher.DispatchAsync(new GetEntityByIdQuery<FileEntry> { Id = id });

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, fileEntry, Operations.Read);
        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }

        var fileEntryText = _fileEntryTextRepository.GetQueryableSet()
              .Where(x => x.FileEntryId == fileEntry.Id)
              .Select(x => new FileEntryTextModel
              {
                  TextLocation = x.TextLocation
              }).FirstOrDefault();

        var stream = System.IO.File.OpenRead(Path.Combine(_options.Storage.TempFolderPath, fileEntryText.TextLocation));
        var ext = Path.GetExtension(fileEntryText.TextLocation).ToLowerInvariant();
        return File(stream, MediaTypeNames.Application.Octet, WebUtility.HtmlEncode(fileEntry.FileName + ext));
    }

    [Authorize(Permissions.DownloadFile)]
    [HttpGet("{id}/downloadchunk/{chunkName}")]
    public async Task<IActionResult> DownloadChunk(Guid id, string chunkName)
    {
        var fileEntry = await _dispatcher.DispatchAsync(new GetEntityByIdQuery<FileEntry> { Id = id });

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, fileEntry, Operations.Read);
        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }

        var fileEntryEmbedding = _fileEntryEmbeddingRepository.GetQueryableSet()
              .Where(x => x.FileEntryId == fileEntry.Id && x.ChunkName == chunkName)
              .Select(x => new FileEntryEmbeddingModel
              {
                  ChunkLocation = x.ChunkLocation,
              }).FirstOrDefault();

        var stream = System.IO.File.OpenRead(Path.Combine(_options.Storage.TempFolderPath, fileEntryEmbedding.ChunkLocation));
        return File(stream, MediaTypeNames.Application.Octet, WebUtility.HtmlEncode(fileEntryEmbedding.ChunkName));
    }

    [Authorize(Permissions.UpdateFile)]
    [HttpPut("{id}")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Put(Guid id, [FromBody] FileEntryModel model)
    {
        var fileEntry = await _dispatcher.DispatchAsync(new GetEntityByIdQuery<FileEntry> { Id = id, ThrowNotFoundIfNull = true });

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, fileEntry, Operations.Update);
        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }

        fileEntry.Name = model.Name;
        fileEntry.Description = model.Description;

        await _dispatcher.DispatchAsync(new AddOrUpdateEntityCommand<FileEntry>(fileEntry));

        return Ok(model);
    }

    [Authorize(Permissions.DeleteFile)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var fileEntry = await _dispatcher.DispatchAsync(new GetEntityByIdQuery<FileEntry> { Id = id });

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, fileEntry, Operations.Delete);
        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }

        fileEntry.Deleted = true;
        fileEntry.DeletedDate = DateTimeOffset.Now;

        await _dispatcher.DispatchAsync(new AddOrUpdateEntityCommand<FileEntry>(fileEntry));

        return Ok();
    }

    [Authorize(Permissions.GetFileAuditLogs)]
    [HttpGet("{id}/auditlogs")]
    public async Task<ActionResult<IEnumerable<AuditLogEntryDTO>>> GetAuditLogs(Guid id)
    {
        var logs = await _dispatcher.DispatchAsync(new GetAuditEntriesQuery { ObjectId = id.ToString() });

        List<dynamic> entries = new List<dynamic>();
        FileEntry previous = null;
        foreach (var log in logs.OrderBy(x => x.CreatedDateTime))
        {
            var data = JsonSerializer.Deserialize<FileEntry>(log.Log);
            var highLight = new
            {
                Name = previous != null && data.Name != previous.Name,
                Description = previous != null && data.Description != previous.Description,
                FileName = previous != null && data.FileName != previous.FileName,
                FileLocation = previous != null && data.FileLocation != previous.FileLocation,
            };

            var entry = new
            {
                log.Id,
                log.UserName,
                Action = log.Action.Replace("_FILEENTRY", string.Empty),
                log.CreatedDateTime,
                data,
                highLight,
            };
            entries.Add(entry);

            previous = data;
        }

        return Ok(entries.OrderByDescending(x => x.CreatedDateTime));
    }
}
