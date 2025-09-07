using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Messaging.AzureServiceBus;

public class AzureServiceBusQueueHealthCheck : IHealthCheck
{
    private readonly string _connectionString;
    private readonly string _queueName;

    public AzureServiceBusQueueHealthCheck(string connectionString, string queueName)
    {
        _connectionString = connectionString;
        _queueName = queueName;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = new ServiceBusAdministrationClient(_connectionString);
            var queue = await client.GetQueueAsync(_queueName, cancellationToken);

            if (string.Equals(queue?.Value?.Name, _queueName, StringComparison.OrdinalIgnoreCase))
            {
                return HealthCheckResult.Healthy();
            }

            return new HealthCheckResult(context.Registration.FailureStatus, description: $"Queue: '{_queueName}' doesn't exist");
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }
}
