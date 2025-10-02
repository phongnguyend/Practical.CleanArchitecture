using ClassifiedAds.Infrastructure.Messaging.AzureServiceBus;
using ClassifiedAds.Infrastructure.Messaging.Kafka;
using ClassifiedAds.Infrastructure.Messaging.RabbitMQ;

namespace ClassifiedAds.Infrastructure.Messaging;

public class MessagingOptions
{
    public string Provider { get; set; }

    public RabbitMQOptions RabbitMQ { get; set; }

    public KafkaOptions Kafka { get; set; }

    public AzureServiceBusOptions AzureServiceBus { get; set; }

    public bool UsedRabbitMQ()
    {
        return Provider == "RabbitMQ";
    }

    public bool UsedKafka()
    {
        return Provider == "Kafka";
    }

    public bool UsedAzureServiceBus()
    {
        return Provider == "AzureServiceBus";
    }

    public bool UsedFake()
    {
        return Provider == "Fake";
    }
}
