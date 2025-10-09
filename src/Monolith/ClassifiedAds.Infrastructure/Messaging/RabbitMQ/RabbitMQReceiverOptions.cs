namespace ClassifiedAds.Infrastructure.Messaging.RabbitMQ;

public class RabbitMQReceiverOptions
{
    public string HostName { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public string QueueName { get; set; }

    public bool AutomaticCreateEnabled { get; set; }

    public string QueueType { get; set; }

    public string ExchangeName { get; set; }

    public string RoutingKey { get; set; }

    public bool SingleActiveConsumer { get; set; }

    public string MessageEncryptionKey { get; set; }

    public int MaxRetryCount { get; set; }

    public int[] RetryIntervals { get; set; }

    public bool DeadLetterEnabled { get; set; }
}
