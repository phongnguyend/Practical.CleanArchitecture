using ClassifiedAds.DomainServices.Entities;
using ClassifiedAds.DomainServices.Infrastructure.Storages;
using System.IO;

namespace ClassifiedAds.Infrastructure.Storages.AzureFile
{
    public class FileManager : IFileStorageManager
    {
        public void Create(FileEntry fileEntry, MemoryStream stream)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(FileEntry fileEntry)
        {
            throw new System.NotImplementedException();
        }

        public byte[] Read(FileEntry fileEntry)
        {
            throw new System.NotImplementedException();
        }
    }
}
