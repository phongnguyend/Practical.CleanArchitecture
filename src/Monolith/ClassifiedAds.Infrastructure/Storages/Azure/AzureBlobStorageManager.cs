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
        private readonly AzureBlobOption _option;
        private readonly CloudBlobContainer _container;

        public AzureBlobStorageManager(AzureBlobOption option)
        {
            _option = option;
            var storageAccount = CloudStorageAccount.Parse(_option.ConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            _container = blobClient.GetContainerReference(_option.Container);
        }

        public async Task CreateAsync(IFileEntry fileEntry, Stream stream, CancellationToken cancellationToken = default)
        {
            await _container.CreateIfNotExistsAsync(cancellationToken);

            CloudBlockBlob blob = _container.GetBlockBlobReference(fileEntry.FileLocation);
            await blob.UploadFromStreamAsync(stream, cancellationToken);
        }

        public async Task DeleteAsync(IFileEntry fileEntry, CancellationToken cancellationToken = default)
        {
            CloudBlockBlob blob = _container.GetBlockBlobReference(fileEntry.FileLocation);
            await blob.DeleteAsync(cancellationToken);
        }

        public async Task<byte[]> ReadAsync(IFileEntry fileEntry, CancellationToken cancellationToken = default)
        {
            CloudBlockBlob blob = _container.GetBlockBlobReference(fileEntry.FileLocation);
            using var stream = new MemoryStream();
            await blob.DownloadToStreamAsync(stream, cancellationToken);
            return stream.ToArray();
        }

        public async Task ArchiveAsync(IFileEntry fileEntry, CancellationToken cancellationToken = default)
        {
            CloudBlockBlob blob = _container.GetBlockBlobReference(fileEntry.FileLocation);
            await blob.SetStandardBlobTierAsync(StandardBlobTier.Cool, cancellationToken);
        }

        public async Task UnArchiveAsync(IFileEntry fileEntry, CancellationToken cancellationToken = default)
        {
            CloudBlockBlob blob = _container.GetBlockBlobReference(fileEntry.FileLocation);
            await blob.SetStandardBlobTierAsync(StandardBlobTier.Hot, cancellationToken);
        }
    }
}
