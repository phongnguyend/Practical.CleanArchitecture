using System;
using System.IO;

namespace ClassifiedAds.Infrastructure.Storages
{
    public interface IFileStorageManager
    {
        void Create(FileEntryDTO fileEntry, MemoryStream stream);
        byte[] Read(FileEntryDTO fileEntry);
        void Delete(FileEntryDTO fileEntry);
    }

    public class FileEntryDTO
    {
        public Guid Id { get; set; }

        public string FileName { get; set; }

        public string FileLocation { get; set; }
    }
}
