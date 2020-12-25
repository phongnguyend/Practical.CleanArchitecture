using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Infrastructure.Storages;
using System.IO;

namespace ClassifiedAds.Infrastructure.Storages.Local
{
    public class LocalFileStorageManager : IFileStorageManager
    {
        private readonly string _rootPath;

        public LocalFileStorageManager(string rootPath)
        {
            _rootPath = rootPath;
        }

        public void Create(FileEntry fileEntry, Stream stream)
        {
            var trustedFileNameForFileStorage = fileEntry.Id.ToString();
            var filePath = Path.Combine(_rootPath, trustedFileNameForFileStorage);

            using (var fileStream = File.Create(filePath))
            {
                stream.CopyTo(fileStream);
            }

            fileEntry.FileLocation = trustedFileNameForFileStorage;
        }

        public void Delete(FileEntry fileEntry)
        {
            var path = Path.Combine(_rootPath, fileEntry.FileLocation);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public byte[] Read(FileEntry fileEntry)
        {
            return File.ReadAllBytes(Path.Combine(_rootPath, fileEntry.FileLocation));
        }
    }
}
