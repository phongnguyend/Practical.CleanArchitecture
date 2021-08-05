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

        public async Task CreateAsync(IFileEntry fileEntry, Stream stream, CancellationToken cancellationToken = default)
        {
            var filePath = Path.Combine(_rootPath, fileEntry.FileLocation);

            var folder = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            using (var fileStream = File.Create(filePath))
            {
                await stream.CopyToAsync(fileStream, cancellationToken);
            }
        }

        public async Task DeleteAsync(IFileEntry fileEntry, CancellationToken cancellationToken = default)
        {
            await Task.Run(() =>
             {
                 var path = Path.Combine(_rootPath, fileEntry.FileLocation);
                 if (File.Exists(path))
                 {
                     File.Delete(path);
                 }
             }, cancellationToken);
        }

        public Task<byte[]> ReadAsync(IFileEntry fileEntry, CancellationToken cancellationToken = default)
        {
            return File.ReadAllBytesAsync(Path.Combine(_rootPath, fileEntry.FileLocation), cancellationToken);
        }
    }
}
