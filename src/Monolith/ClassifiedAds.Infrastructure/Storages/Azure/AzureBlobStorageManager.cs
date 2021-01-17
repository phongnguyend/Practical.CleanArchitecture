using ClassifiedAds.Domain.Infrastructure.Storages;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Storages.Azure
{
    public class AzureBlobStorageManager : IFileStorageManager
    {
        private readonly string _connectionString;
        private readonly string _containerName;
        private readonly CloudBlobContainer _container;

        public AzureBlobStorageManager(string connectionString, string containerName)
        {
            _connectionString = connectionString;
            _containerName = containerName;

            var storageAccount = CloudStorageAccount.Parse(_connectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            _container = blobClient.GetContainerReference(_containerName);
        }

        public void Create(IFileEntry fileEntry, Stream stream)
        {
            CreateAsync(fileEntry, stream).GetAwaiter().GetResult();
        }

        public async Task CreateAsync(IFileEntry fileEntry, Stream stream, CancellationToken cancellationToken = default)
        {
            _container.CreateIfNotExistsAsync().GetAwaiter().GetResult();

            var name = fileEntry.Id.ToString();
            CloudBlockBlob blob = _container.GetBlockBlobReference(name);
            await blob.UploadFromStreamAsync(stream);

            fileEntry.FileLocation = name;
        }

        public void Delete(IFileEntry fileEntry)
        {
            DeleteAsync(fileEntry).GetAwaiter().GetResult();
        }

        public async Task DeleteAsync(IFileEntry fileEntry, CancellationToken cancellationToken = default)
        {
            CloudBlockBlob blob = _container.GetBlockBlobReference(fileEntry.FileLocation);
            await blob.DeleteAsync();
        }

        public byte[] Read(IFileEntry fileEntry)
        {
            return ReadAsync(fileEntry).GetAwaiter().GetResult();
        }

        public async Task<byte[]> ReadAsync(IFileEntry fileEntry, CancellationToken cancellationToken = default)
        {
            CloudBlockBlob blob = _container.GetBlockBlobReference(fileEntry.FileLocation);
            using (var stream = new MemoryStream())
            {
                await blob.DownloadToStreamAsync(stream);
                return stream.ToArray();
            }
        }
    }
}
