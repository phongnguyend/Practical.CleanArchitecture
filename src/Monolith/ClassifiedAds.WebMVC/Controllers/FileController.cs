﻿using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using ClassifiedAds.Application;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Infrastructure.Storages;
using ClassifiedAds.WebMVC.Models.File;
using Microsoft.AspNetCore.Mvc;

namespace ClassifiedAds.WebMVC.Controllers
{
    public class FileController : Controller
    {
        private readonly Dispatcher _dispatcher;
        private readonly IFileStorageManager _fileManager;

        public FileController(Dispatcher dispatcher, IFileStorageManager fileManager)
        {
            _dispatcher = dispatcher;
            _fileManager = fileManager;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _dispatcher.DispatchAsync(new GetEntititesQuery<FileEntry>()));
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(UploadFile model)
        {
            var fileEntry = new FileEntry
            {
                Name = model.Name,
                Description = model.Description,
                Size = model.FormFile.Length,
                UploadedTime = DateTime.Now,
                FileName = model.FormFile.FileName,
            };

            await _dispatcher.DispatchAsync(new AddOrUpdateEntityCommand<FileEntry>(fileEntry));

            using (var stream = new MemoryStream())
            {
                await model.FormFile.CopyToAsync(stream);
                await _fileManager.CreateAsync(fileEntry, stream);
            }

            await _dispatcher.DispatchAsync(new AddOrUpdateEntityCommand<FileEntry>(fileEntry));

            return View();
        }

        public async Task<IActionResult> Download(Guid id)
        {
            var fileEntry = await _dispatcher.DispatchAsync(new GetEntityByIdQuery<FileEntry> { Id = id });
            var content = await _fileManager.ReadAsync(fileEntry);
            return File(content, MediaTypeNames.Application.Octet, WebUtility.HtmlEncode(fileEntry.FileName));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var fileEntry = await _dispatcher.DispatchAsync(new GetEntityByIdQuery<FileEntry> { Id = id });
            return View(fileEntry);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(FileEntry model)
        {
            var fileEntry = await _dispatcher.DispatchAsync(new GetEntityByIdQuery<FileEntry> { Id = model.Id });

            await _dispatcher.DispatchAsync(new DeleteEntityCommand<FileEntry> { Entity = fileEntry });
            await _fileManager.DeleteAsync(fileEntry);

            return RedirectToAction(nameof(Index));
        }
    }
}