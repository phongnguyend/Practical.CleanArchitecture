using Azure.Storage.Blobs;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Storages.Azure;

public class AzureBlobStorageHealthCheck : IHealthCheck
{
    private readonly AzureBlobOption _option;
    private readonly BlobContainerClient _container;

    public AzureBlobStorageHealthCheck(AzureBlobOption option)
    {
        _option = option;
        _container = new BlobContainerClient(_option.ConnectionString, _option.Container);
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
    {
        try
        {
            var fileName = _option.Path + $"HealthCheck/{DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss")}-{Guid.NewGuid()}.txt";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes($"HealthCheck {DateTime.Now}"));
            await _container.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
            BlobClient blob = _container.GetBlobClient(fileName);
            await blob.UploadAsync(stream, overwrite: true, cancellationToken);
            await blob.DeleteAsync(cancellationToken: cancellationToken);

            return HealthCheckResult.Healthy($"ContainerName: {_option.Container}");
        }
        catch (Exception exception)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, null, exception);
        }
    }
}
