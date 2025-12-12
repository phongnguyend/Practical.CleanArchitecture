using ClassifiedAds.Domain.Infrastructure.Messaging;
using ClassifiedAds.Infrastructure.Messaging;
using ClassifiedAds.Infrastructure.Messaging.AzureServiceBus;
using ClassifiedAds.Infrastructure.Messaging.Fake;
using ClassifiedAds.Infrastructure.Messaging.Kafka;
using ClassifiedAds.Infrastructure.Messaging.RabbitMQ;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection;

public static class MessagingCollectionExtensions
{
    public static IServiceCollection AddAzureServiceBusSender<T>(this IServiceCollection services, AzureServiceBusOptions options)
    {
        services.AddSingleton<IMessageSender<T>>(new AzureServiceBusSender<T>(
                            options.ConnectionString,
                            options.QueueNames[typeof(T).Name]));
        return services;
    }

    public static IServiceCollection AddAzureServiceBusReceiver<TConsumer, T>(this IServiceCollection services, AzureServiceBusOptions options)
    {
        services.AddTransient<IMessageReceiver<TConsumer, T>>(x => new AzureServiceBusReceiver<TConsumer, T>(
                            options.ConnectionString,
                            options.QueueNames[typeof(T).Name]));
        return services;
    }

    public static IServiceCollection AddFakeSender<T>(this IServiceCollection services)
    {
        services.AddSingleton<IMessageSender<T>>(new FakeSender<T>());
        return services;
    }

    public static IServiceCollection AddFakeReceiver<TConsumer, T>(this IServiceCollection services)
    {
        services.AddTransient<IMessageReceiver<TConsumer, T>>(x => new FakeReceiver<TConsumer, T>());
        return services;
    }

    public static IServiceCollection AddKafkaSender<T>(this IServiceCollection services, KafkaOptions options)
    {
        services.AddSingleton<IMessageSender<T>>(new KafkaSender<T>(options.BootstrapServers, options.Topics[typeof(T).Name]));
        return services;
    }

    public static IServiceCollection AddKafkaReceiver<TConsumer, T>(this IServiceCollection services, KafkaOptions options)
    {
        services.AddTransient<IMessageReceiver<TConsumer, T>>(x => new KafkaReceiver<TConsumer, T>(options.BootstrapServers,
            options.Topics[typeof(T).Name],
            options.GroupId));
        return services;
    }

    public static IServiceCollection AddRabbitMQSender<T>(this IServiceCollection services, RabbitMQOptions options)
    {
        services.AddSingleton<IMessageSender<T>>(new RabbitMQSender<T>(new RabbitMQSenderOptions
        {
            HostName = options.HostName,
            UserName = options.UserName,
            Password = options.Password,
            ExchangeName = options.ExchangeName,
            RoutingKey = options.RoutingKeys[typeof(T).Name],
            MessageEncryptionEnabled = options.MessageEncryptionEnabled,
            MessageEncryptionKey = options.MessageEncryptionKey
        }));
        return services;
    }

    public static IServiceCollection AddRabbitMQReceiver<TConsumer, T>(this IServiceCollection services, RabbitMQOptions options)
    {
        var receiverOptions = new RabbitMQReceiverOptions
        {
            HostName = options.HostName,
            UserName = options.UserName,
            Password = options.Password,
            ExchangeName = options.ExchangeName,
            RoutingKey = options.RoutingKeys[typeof(T).Name],
            QueueName = options.Consumers[typeof(TConsumer).Name][typeof(T).Name],
            AutomaticCreateEnabled = true,
            MessageEncryptionKey = options.MessageEncryptionKey,
            MaxRetryCount = options.GetMaxRetryCount(typeof(TConsumer).Name),
            RetryIntervals = options.GetRetryIntervals(typeof(TConsumer).Name),
            DeadLetterEnabled = options.GetDeadLetterEnabled(typeof(TConsumer).Name)
        };

        services.AddTransient<IMessageReceiver<TConsumer, T>>(x => new RabbitMQReceiver<TConsumer, T>(receiverOptions, x.GetRequiredService<ILogger<RabbitMQReceiver<TConsumer, T>>>()));
        return services;
    }

    public static IServiceCollection AddMessageBusSender<T>(this IServiceCollection services, MessagingOptions options)
    {
        if (options.UsedRabbitMQ())
        {
            services.AddRabbitMQSender<T>(options.RabbitMQ);
        }
        else if (options.UsedKafka())
        {
            services.AddKafkaSender<T>(options.Kafka);
        }
        else if (options.UsedAzureServiceBus())
        {
            services.AddAzureServiceBusSender<T>(options.AzureServiceBus);
        }
        else if (options.UsedFake())
        {
            services.AddFakeSender<T>();
        }

        return services;
    }

    public static IServiceCollection AddMessageBusReceiver<TConsumer, T>(this IServiceCollection services, MessagingOptions options)
    {
        if (options.UsedRabbitMQ())
        {
            services.AddRabbitMQReceiver<TConsumer, T>(options.RabbitMQ);
        }
        else if (options.UsedKafka())
        {
            services.AddKafkaReceiver<TConsumer, T>(options.Kafka);
        }
        else if (options.UsedAzureServiceBus())
        {
            services.AddAzureServiceBusReceiver<TConsumer, T>(options.AzureServiceBus);
        }
        else if (options.UsedFake())
        {
            services.AddFakeReceiver<TConsumer, T>();
        }

        return services;
    }

    public static IHealthChecksBuilder AddMessageBusHealthCheck(this IHealthChecksBuilder healthChecksBuilder, MessagingOptions options)
    {
        if (options.UsedRabbitMQ())
        {
            var name = "Message Broker (RabbitMQ)";

            healthChecksBuilder.AddRabbitMQ(new RabbitMQHealthCheckOptions
            {
                HostName = options.RabbitMQ.HostName,
                UserName = options.RabbitMQ.UserName,
                Password = options.RabbitMQ.Password,
            },
            name: name,
            failureStatus: HealthStatus.Degraded);
        }
        else if (options.UsedKafka())
        {
            var name = "Message Broker (Kafka)";
            healthChecksBuilder.AddKafka(
                bootstrapServers: options.Kafka.BootstrapServers,
                topic: "healthcheck",
                name: name,
                failureStatus: HealthStatus.Degraded);
        }
        else if (options.UsedAzureServiceBus())
        {
            foreach (var queueName in options.AzureServiceBus.QueueNames)
            {
                healthChecksBuilder.AddAzureServiceBusQueue(
                    connectionString: options.AzureServiceBus.ConnectionString,
                    queueName: queueName.Value,
                    name: $"Message Broker (Azure Service Bus) {queueName.Key}",
                    failureStatus: HealthStatus.Degraded);
            }
        }
        else if (options.UsedFake())
        {
        }

        return healthChecksBuilder;
    }

}
