using ClassifiedAds.Domain.Infrastructure.Storages;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Storages.Local
{
    public class LocalFileStorageManager : IFileStorageManager
    {
        private readonly string _rootPath;

        public LocalFileStorageManager(string rootPath)
        {
            _rootPath = rootPath;
        }

        public void Create(IFileEntry fileEntry, Stream stream)
        {
            CreateAsync(fileEntry, stream).GetAwaiter().GetResult();
        }

        public async Task CreateAsync(IFileEntry fileEntry, Stream stream, CancellationToken cancellationToken = default)
        {
            var trustedFileNameForFileStorage = fileEntry.Id.ToString();
            var filePath = Path.Combine(_rootPath, trustedFileNameForFileStorage);

            using (var fileStream = File.Create(filePath))
            {
                await stream.CopyToAsync(fileStream, cancellationToken);
            }

            fileEntry.FileLocation = trustedFileNameForFileStorage;
        }

        public void Delete(IFileEntry fileEntry)
        {
            var path = Path.Combine(_rootPath, fileEntry.FileLocation);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public Task DeleteAsync(IFileEntry fileEntry, CancellationToken cancellationToken = default)
        {
            Delete(fileEntry);
            return Task.CompletedTask;
        }

        public byte[] Read(IFileEntry fileEntry)
        {
            return ReadAsync(fileEntry).GetAwaiter().GetResult();
        }

        public Task<byte[]> ReadAsync(IFileEntry fileEntry, CancellationToken cancellationToken = default)
        {
            return File.ReadAllBytesAsync(Path.Combine(_rootPath, fileEntry.FileLocation));
        }
    }
}
