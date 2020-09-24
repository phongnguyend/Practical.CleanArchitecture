using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.HealthChecks
{
    public class FilePathWriteHealthCheck : IHealthCheck
    {
        private string _path;

        public FilePathWriteHealthCheck(string path)
        {
            _path = path;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var testFile = $"{_path}\\HealthCheck_{Guid.NewGuid()}.txt";
                var fs = File.Create(testFile);
                fs.Close();
                File.Delete(testFile);

                return Task.FromResult(HealthCheckResult.Healthy($"Application has read and write permissions to {_path}"));
            }
            catch (Exception e)
            {
                if (context.Registration.FailureStatus == HealthStatus.Unhealthy)
                {
                    return Task.FromResult(HealthCheckResult.Unhealthy(e.Message));
                }
                else
                {
                    return Task.FromResult(HealthCheckResult.Degraded(e.Message));
                }
            }

        }
    }
}