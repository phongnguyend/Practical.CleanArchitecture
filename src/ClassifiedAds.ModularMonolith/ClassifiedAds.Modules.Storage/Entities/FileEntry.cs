using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Modules.Storage.DTOs;
using System;

namespace ClassifiedAds.Modules.Storage.Entities
{
    public class FileEntry : AggregateRoot<Guid>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public long Size { get; set; }

        public DateTime UploadedTime { get; set; }

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
