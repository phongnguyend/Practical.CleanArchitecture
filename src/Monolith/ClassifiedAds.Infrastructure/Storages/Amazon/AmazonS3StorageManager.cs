using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using ClassifiedAds.Domain.Infrastructure.Storages;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Storages.Amazon
{
    public class AmazonS3StorageManager : IFileStorageManager
    {
        private readonly IAmazonS3 _client;
        private readonly AmazonOptions _options;

        public AmazonS3StorageManager(AmazonOptions options)
        {
            _client = new AmazonS3Client(options.AccessKeyID, options.SecretAccessKey, RegionEndpoint.GetBySystemName(options.RegionEndpoint));
            _options = options;
        }

        public async Task CreateAsync(IFileEntry fileEntry, Stream stream, CancellationToken cancellationToken = default)
        {
            var fileTransferUtility = new TransferUtility(_client);

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = stream,
                Key = fileEntry.Id.ToString(),
                BucketName = _options.BucketName,
                CannedACL = S3CannedACL.NoACL,
            };

            await fileTransferUtility.UploadAsync(uploadRequest, cancellationToken);

            fileEntry.FileLocation = uploadRequest.Key;

        }

        public async Task DeleteAsync(IFileEntry fileEntry, CancellationToken cancellationToken = default)
        {
            await _client.DeleteObjectAsync(_options.BucketName, fileEntry.FileLocation, cancellationToken);
        }

        public async Task<byte[]> ReadAsync(IFileEntry fileEntry, CancellationToken cancellationToken = default)
        {
            var request = new GetObjectRequest
            {
                BucketName = _options.BucketName,
                Key = fileEntry.FileLocation,
            };

            using var response = await _client.GetObjectAsync(request, cancellationToken);
            using var responseStream = response.ResponseStream;
            using var reader = new MemoryStream();
            await responseStream.CopyToAsync(reader, cancellationToken);
            return reader.ToArray();
        }
    }
}
