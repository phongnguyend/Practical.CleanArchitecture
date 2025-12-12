using System.Collections.Generic;

namespace ClassifiedAds.Infrastructure.Messaging.RabbitMQ;

public class RabbitMQOptions
{
    public string HostName { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public string ExchangeName { get; set; }

    public Dictionary<string, string> RoutingKeys { get; set; }

    public Dictionary<string, Dictionary<string, string>> Consumers { get; set; }

    public string ConnectionString
    {
        get
        {
            return $"amqp://{UserName}:{Password}@{HostName}/%2f";
        }
    }

    public bool MessageEncryptionEnabled { get; set; }

    public string MessageEncryptionKey { get; set; }

    public string TryGetProperty(string consumerName, string propertyName)
    {
        return Consumers[consumerName].TryGetValue(propertyName, out var value) ? value : null;
    }

    public int GetMaxRetryCount(string consumerName)
    {
        return int.TryParse(TryGetProperty(consumerName, "Property_MaxRetryCount"), out var maxRetryCount) ? maxRetryCount : 0;
    }

    public bool GetDeadLetterEnabled(string consumerName)
    {
        return bool.TryParse(TryGetProperty(consumerName, "Property_DeadLetterEnabled"), out var deadLetterEnabled) ? deadLetterEnabled : false;
    }
}
