using ClassifiedAds.Domain.Entities;
using System.IO;

namespace ClassifiedAds.Domain.Infrastructure.Storages
{
    public interface IFileStorageManager
    {
        void Create(FileEntry fileEntry, Stream stream);
        byte[] Read(FileEntry fileEntry);
        void Delete(FileEntry fileEntry);
    }
}
