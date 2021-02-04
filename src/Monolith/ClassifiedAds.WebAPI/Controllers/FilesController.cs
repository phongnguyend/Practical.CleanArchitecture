﻿using ClassifiedAds.Application;
using ClassifiedAds.Application.AuditLogEntries.DTOs;
using ClassifiedAds.Application.AuditLogEntries.Queries;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Infrastructure.Storages;
using ClassifiedAds.Infrastructure.Web.Authorization.Policies;
using ClassifiedAds.WebAPI.Authorization.Policies.Files;
using ClassifiedAds.WebAPI.Models.Files;
using CryptographyHelper;
using CryptographyHelper.SymmetricAlgorithms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ClassifiedAds.WebAPI.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : Controller
    {
        private readonly Dispatcher _dispatcher;
        private readonly IFileStorageManager _fileManager;
        private readonly IMemoryCache _memoryCache;

        public FilesController(Dispatcher dispatcher, IFileStorageManager fileManager, IMemoryCache memoryCache)
        {
            _dispatcher = dispatcher;
            _fileManager = fileManager;
            _memoryCache = memoryCache;
        }

        [AuthorizePolicy(typeof(GetFilesPolicy))]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FileEntryModel>>> Get()
        {
            var fileEntries = await _dispatcher.DispatchAsync(new GetEntititesQuery<FileEntry>());
            return Ok(fileEntries.ToModels());
        }

        [AuthorizePolicy(typeof(UploadFilePolicy))]
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

            if (model.Encrypted)
            {
                var key = SymmetricCrypto.GenerateKey(16);
                using (var inputStream = model.FormFile.OpenReadStream())
                using (var encryptedStream = new MemoryStream(inputStream
                        .UseAES(key)
                        .WithCipher(CipherMode.ECB)
                        .WithPadding(PaddingMode.PKCS7)
                        .Encrypt()))
                {
                    await _fileManager.CreateAsync(fileEntry, encryptedStream);
                }

                fileEntry.EncryptionKey = key.ToBase64String();
            }
            else
            {
                using (var stream = model.FormFile.OpenReadStream())
                {
                    await _fileManager.CreateAsync(fileEntry, stream);
                }
            }

            await _dispatcher.DispatchAsync(new AddOrUpdateEntityCommand<FileEntry>(fileEntry));

            return Ok(fileEntry.ToModel());
        }

        [AuthorizePolicy(typeof(GetFilePolicy))]
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<FileEntryModel>>> Get(Guid id)
        {
            var fileEntry = await _dispatcher.DispatchAsync(new GetEntityByIdQuery<FileEntry> { Id = id });
            return Ok(fileEntry.ToModel());
        }

        [AuthorizePolicy(typeof(DownloadFilePolicy))]
        [HttpGet("{id}/download")]
        public async Task<IActionResult> Download(Guid id)
        {
            var fileEntry = await _dispatcher.DispatchAsync(new GetEntityByIdQuery<FileEntry> { Id = id });

            var rawData = await _fileManager.ReadAsync(fileEntry);
            var content = fileEntry.Encrypted && fileEntry.FileLocation != "Fake.txt"
                ? rawData
                .UseAES(fileEntry.EncryptionKey.FromBase64String())
                .WithCipher(CipherMode.ECB)
                .WithPadding(PaddingMode.PKCS7)
                .Decrypt()
                : rawData;

            return File(content, MediaTypeNames.Application.Octet, WebUtility.HtmlEncode(fileEntry.FileName));
        }

        [AuthorizePolicy(typeof(UpdateFilePolicy))]
        [HttpPut("{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Put(Guid id, [FromBody] FileEntryModel model)
        {
            var fileEntry = await _dispatcher.DispatchAsync(new GetEntityByIdQuery<FileEntry> { Id = id, ThrowNotFoundIfNull = true });

            fileEntry.Name = model.Name;
            fileEntry.Description = model.Description;

            await _dispatcher.DispatchAsync(new AddOrUpdateEntityCommand<FileEntry>(fileEntry));

            return Ok(model);
        }

        [AuthorizePolicy(typeof(DeleteFilePolicy))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var fileEntry = await _dispatcher.DispatchAsync(new GetEntityByIdQuery<FileEntry> { Id = id });

            await _dispatcher.DispatchAsync(new DeleteEntityCommand<FileEntry> { Entity = fileEntry });
            await _fileManager.DeleteAsync(fileEntry);

            return Ok();
        }

        [AuthorizePolicy(typeof(GetFileAuditLogsPolicy))]
        [HttpGet("{id}/auditlogs")]
        public async Task<ActionResult<IEnumerable<AuditLogEntryDTO>>> GetAuditLogs(Guid id)
        {
            var logs = await _dispatcher.DispatchAsync(new GetAuditEntriesQuery { ObjectId = id.ToString() });

            List<dynamic> entries = new List<dynamic>();
            FileEntry previous = null;
            foreach (var log in logs.OrderBy(x => x.CreatedDateTime))
            {
                var data = JsonConvert.DeserializeObject<FileEntry>(log.Log);
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
}
