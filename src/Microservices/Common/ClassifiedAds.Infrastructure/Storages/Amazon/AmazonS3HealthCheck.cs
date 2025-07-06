using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Storages.Amazon;

public class AmazonS3HealthCheck : IHealthCheck
{
    private readonly IAmazonS3 _client;
    private readonly AmazonOptions _options;

    public AmazonS3HealthCheck(AmazonOptions options)
    {
        _client = options.CreateAmazonS3Client();
        _options = options;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var fileName = _options.Path + $"HealthCheck/{DateTime.Now:yyyy-MM-dd-hh-mm-ss}-{Guid.NewGuid()}.txt";
            var fileTransferUtility = new TransferUtility(_client);

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes($"HealthCheck {DateTime.Now}"));
            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = stream,
                Key = fileName,
                BucketName = _options.BucketName,
                CannedACL = S3CannedACL.NoACL,
            };

            await fileTransferUtility.UploadAsync(uploadRequest, cancellationToken);
            await _client.DeleteObjectAsync(_options.BucketName, fileName, cancellationToken);

            return HealthCheckResult.Healthy($"BucketName: {_options.BucketName}");
        }
        catch (Exception exception)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, null, exception);
        }
    }
}
