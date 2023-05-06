using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Storages.Local;

public class LocalFileHealthCheck : IHealthCheck
{
    private readonly LocalFileHealthCheckOptions _options;

    public LocalFileHealthCheck(LocalFileHealthCheckOptions options)
    {
        _options = options;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var testFile = $"{_options.Path}\\HealthCheck_{Guid.NewGuid()}.txt";
            using (var fs = File.Create(testFile))
            {
            }

            File.Delete(testFile);

            return Task.FromResult(HealthCheckResult.Healthy($"Path: {_options.Path}"));
        }
        catch (Exception ex)
        {
            if (context.Registration.FailureStatus == HealthStatus.Unhealthy)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(ex.Message));
            }
            else
            {
                return Task.FromResult(HealthCheckResult.Degraded(ex.Message, ex));
            }
        }
    }
}

public class LocalFileHealthCheckOptions : LocalOptions
{
}
