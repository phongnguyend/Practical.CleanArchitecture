using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Infrastructure.Storages;
using ClassifiedAds.Domain.Services;
using ClassifiedAds.WebMVC.Models.File;
using Microsoft.AspNetCore.Mvc;

namespace ClassifiedAds.WebMVC.Controllers
{
    public class FileController : Controller
    {
        private readonly ICrudService<FileEntry> _fileEntryService;
        private readonly IFileStorageManager _fileManager;

        public FileController(ICrudService<FileEntry> fileEntryService, IFileStorageManager fileManager)
        {
            _fileEntryService = fileEntryService;
            _fileManager = fileManager;
        }

        public IActionResult Index()
        {
            return View(_fileEntryService.Get());
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

            _fileEntryService.AddOrUpdate(fileEntry);

            using (var stream = new MemoryStream())
            {
                await model.FormFile.CopyToAsync(stream);
                _fileManager.Create(fileEntry, stream);
            }

            _fileEntryService.AddOrUpdate(fileEntry);

            return View();
        }

        public IActionResult Download(Guid id)
        {
            var fileEntry = _fileEntryService.GetById(id);
            var content = _fileManager.Read(fileEntry);
            return File(content, MediaTypeNames.Application.Octet, WebUtility.HtmlEncode(fileEntry.FileName));
        }

        [HttpGet]
        public IActionResult Delete(Guid id)
        {
            var fileEntry = _fileEntryService.GetById(id);
            return View(fileEntry);
        }

        [HttpPost]
        public IActionResult Delete(FileEntry model)
        {
            var fileEntry = _fileEntryService.GetById(model.Id);

            _fileEntryService.Delete(fileEntry);
            _fileManager.Delete(fileEntry);

            return RedirectToAction(nameof(Index));
        }
    }
}