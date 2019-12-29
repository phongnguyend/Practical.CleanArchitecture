using ClassifiedAds.DomainServices.Entities;
using System.IO;

namespace ClassifiedAds.DomainServices.Infrastructure.Storages
{
    public interface IFileStorageManager
    {
        void Create(FileEntry fileEntry, MemoryStream stream);
        byte[] Read(FileEntry fileEntry);
        void Delete(FileEntry fileEntry);
    }
}
