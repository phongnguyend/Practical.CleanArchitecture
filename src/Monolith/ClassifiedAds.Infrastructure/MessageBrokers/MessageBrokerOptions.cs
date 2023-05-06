using ClassifiedAds.Infrastructure.MessageBrokers.AzureEventGrid;
using ClassifiedAds.Infrastructure.MessageBrokers.AzureEventHub;
using ClassifiedAds.Infrastructure.MessageBrokers.AzureQueue;
using ClassifiedAds.Infrastructure.MessageBrokers.AzureServiceBus;
using ClassifiedAds.Infrastructure.MessageBrokers.Kafka;
using ClassifiedAds.Infrastructure.MessageBrokers.RabbitMQ;

namespace ClassifiedAds.Infrastructure.MessageBrokers;

public class MessageBrokerOptions
{
    public string Provider { get; set; }

    public RabbitMQOptions RabbitMQ { get; set; }

    public KafkaOptions Kafka { get; set; }

    public AzureQueueOptions AzureQueue { get; set; }

    public AzureServiceBusOptions AzureServiceBus { get; set; }

    public AzureEventGridOptions AzureEventGrid { get; set; }

    public AzureEventHubOptions AzureEventHub { get; set; }

    public bool UsedRabbitMQ()
    {
        return Provider == "RabbitMQ";
    }

    public bool UsedKafka()
    {
        return Provider == "Kafka";
    }

    public bool UsedAzureQueue()
    {
        return Provider == "AzureQueue";
    }

    public bool UsedAzureServiceBus()
    {
        return Provider == "AzureServiceBus";
    }

    public bool UsedAzureEventGrid()
    {
        return Provider == "AzureEventGrid";
    }

    public bool UsedAzureEventHub()
    {
        return Provider == "AzureEventHub";
    }

    public bool UsedFake()
    {
        return Provider == "Fake";
    }
}
