using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Services.Storage.DTOs;
using System;

namespace ClassifiedAds.Services.Storage.Entities
{
    public class FileEntry : AggregateRoot<Guid>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public long Size { get; set; }

        public DateTimeOffset UploadedTime { get; set; }

        public string FileName { get; set; }

        public string FileLocation { get; set; }

        public FileEntryDTO ToFileEntryDTO()
        {
            return new FileEntryDTO
            {
                Id = Id,
                FileName = FileName,
                FileLocation = FileLocation,
            };
        }
    }
}
