using ClassifiedAds.Modules.Storage.DTOs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;

namespace ClassifiedAds.Modules.Storage.Storages.Azure
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

        public void Create(FileEntryDTO fileEntry, MemoryStream stream)
        {
            _container.CreateIfNotExistsAsync().GetAwaiter().GetResult();

            var name = fileEntry.Id.ToString();
            CloudBlockBlob blob = _container.GetBlockBlobReference(name);
            var bytes = stream.ToArray();
            blob.UploadFromByteArrayAsync(bytes, 0, bytes.Length).Wait();

            fileEntry.FileLocation = name;
        }

        public void Delete(FileEntryDTO fileEntry)
        {
            CloudBlockBlob blob = _container.GetBlockBlobReference(fileEntry.FileLocation);
            blob.DeleteAsync().Wait();
        }

        public byte[] Read(FileEntryDTO fileEntry)
        {
            CloudBlockBlob blob = _container.GetBlockBlobReference(fileEntry.FileLocation);
            using (var stream = new MemoryStream())
            {
                blob.DownloadToStreamAsync(stream).Wait();
                return stream.ToArray();
            }
        }
    }
}
