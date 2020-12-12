using ClassifiedAds.Application;
using ClassifiedAds.Infrastructure.Storages;
using ClassifiedAds.Modules.AuditLog.Contracts.DTOs;
using ClassifiedAds.Modules.Storage.Entities;
using ClassifiedAds.Modules.Storage.Queries;
using CryptographyHelper;
using CryptographyHelper.SymmetricAlgorithms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.Storage.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : Controller
    {
        private readonly Dispatcher _dispatcher;
        private readonly IFileStorageManager _fileManager;

        public FilesController(Dispatcher dispatcher, IFileStorageManager fileManager)
        {
            _dispatcher = dispatcher;
            _fileManager = fileManager;
        }

        [HttpGet]
        public ActionResult<IEnumerable<FileEntry>> Get()
        {
            return Ok(_dispatcher.Dispatch(new GetEntititesQuery<FileEntry>()));
        }

        [HttpPost]
        public async Task<ActionResult<FileEntry>> Upload([FromForm] UploadFile model)
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

            _dispatcher.Dispatch(new AddOrUpdateEntityCommand<FileEntry>(fileEntry));

            var fileEntryDTO = fileEntry.ToFileEntryDTO();

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
                    _fileManager.Create(fileEntryDTO, encryptedStream);
                }

                fileEntry.EncryptionKey = key.ToBase64String();
            }
            else
            {
                using (var stream = model.FormFile.OpenReadStream())
                {
                    _fileManager.Create(fileEntryDTO, stream);
                }
            }

            fileEntry.FileLocation = fileEntryDTO.FileLocation;

            _dispatcher.Dispatch(new AddOrUpdateEntityCommand<FileEntry>(fileEntry));

            return Ok(fileEntry);
        }

        [HttpGet("{id}")]
        public ActionResult<IEnumerable<FileEntry>> Get(Guid id)
        {
            return Ok(_dispatcher.Dispatch(new GetEntityByIdQuery<FileEntry> { Id = id }));
        }

        [HttpGet("{id}/download")]
        public IActionResult Download(Guid id)
        {
            var fileEntry = _dispatcher.Dispatch(new GetEntityByIdQuery<FileEntry> { Id = id });

            var rawData = _fileManager.Read(fileEntry.ToFileEntryDTO());
            var content = fileEntry.Encrypted
                ? rawData
                .UseAES(fileEntry.EncryptionKey.FromBase64String())
                .WithCipher(CipherMode.ECB)
                .WithPadding(PaddingMode.PKCS7)
                .Decrypt()
                : rawData;

            return File(content, MediaTypeNames.Application.Octet, WebUtility.HtmlEncode(fileEntry.FileName));
        }

        [HttpPut("{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Put(Guid id, [FromBody] FileEntry model)
        {
            var fileEntry = _dispatcher.Dispatch(new GetEntityByIdQuery<FileEntry> { Id = id, ThrowNotFoundIfNull = true });

            fileEntry.Name = model.Name;
            fileEntry.Description = model.Description;

            _dispatcher.Dispatch(new AddOrUpdateEntityCommand<FileEntry>(fileEntry));

            return Ok(model);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var fileEntry = _dispatcher.Dispatch(new GetEntityByIdQuery<FileEntry> { Id = id });

            _dispatcher.Dispatch(new DeleteEntityCommand<FileEntry> { Entity = fileEntry });
            _fileManager.Delete(fileEntry.ToFileEntryDTO());

            return Ok();
        }

        [HttpGet("{id}/auditlogs")]
        public ActionResult<IEnumerable<AuditLogEntryDTO>> GetAuditLogs(Guid id)
        {
            var logs = _dispatcher.Dispatch(new GetAuditEntriesQuery { ObjectId = id.ToString() });

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

    public class UploadFile
    {
        [Display(Name = "Name")]
        [StringLength(50, MinimumLength = 0)]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [StringLength(50, MinimumLength = 0)]
        public string Description { get; set; }

        [Required]
        [Display(Name = "File")]
        public IFormFile FormFile { get; set; }

        public bool Encrypted { get; set; }
    }
}
