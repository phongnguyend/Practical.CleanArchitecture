using Confluent.Kafka;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Messaging.Kafka;

public class KafkaHealthCheck : IHealthCheck
{
    private readonly string _bootstrapServers;
    private readonly string _topic;

    public KafkaHealthCheck(string bootstrapServers, string topic)
    {
        _bootstrapServers = bootstrapServers;
        _topic = topic;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
    {
        try
        {
            var config = new ProducerConfig { BootstrapServers = _bootstrapServers, MessageTimeoutMs = 10000 };

            using var producer = new ProducerBuilder<Null, string>(config).Build();
            var result = await producer.ProduceAsync(_topic, new Message<Null, string>
            {
                Value = $"Health Check {DateTimeOffset.Now}",
            }, cancellationToken);

            if (result.Status == PersistenceStatus.NotPersisted)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, "Message was never transmitted to the broker, or failed with an error indicating it was not written to the log. Application retry risks ordering, but not duplication");
            }

            return HealthCheckResult.Healthy();
        }
        catch (Exception exception)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, exception: exception);
        }
    }
}
