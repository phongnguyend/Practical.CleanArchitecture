using Azure.Storage.Queues;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.MessageBrokers.AzureQueue;

public class AzureQueueStorageHealthCheck : IHealthCheck
{
    private readonly string _connectionString;
    private readonly string _queueName;

    public AzureQueueStorageHealthCheck(string connectionString, string queueName)
    {
        _connectionString = connectionString;
        _queueName = queueName;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var queueClient = new QueueClient(_connectionString, _queueName);

            if (!await queueClient.ExistsAsync(cancellationToken))
            {
                return new HealthCheckResult(context.Registration.FailureStatus, description: $"Queue '{_queueName}' not exists");
            }

            await queueClient.GetPropertiesAsync(cancellationToken);

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }
}
