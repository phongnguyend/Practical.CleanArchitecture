using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Messaging.RabbitMQ;

public class RabbitMQHealthCheck : IHealthCheck
{
    private readonly RabbitMQHealthCheckOptions _options;

    public RabbitMQHealthCheck(RabbitMQHealthCheckOptions options)
    {
        _options = options;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = _options.HostName,
                UserName = _options.UserName,
                Password = _options.Password,
            };

            using var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
            using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            return HealthCheckResult.Healthy($"HostName: {_options.HostName}");
        }
        catch (Exception exception)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, null, exception);
        }
    }
}

public class RabbitMQHealthCheckOptions
{
    public string HostName { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }
}
