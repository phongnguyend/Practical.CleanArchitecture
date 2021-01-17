using ClassifiedAds.Infrastructure.Storages;
using System;

namespace ClassifiedAds.Services.Storage.DTOs
{
    public class FileEntryDTO : IFileEntry
    {
        public Guid Id { get; set; }

        public string FileName { get; set; }

        public string FileLocation { get; set; }
    }
}
