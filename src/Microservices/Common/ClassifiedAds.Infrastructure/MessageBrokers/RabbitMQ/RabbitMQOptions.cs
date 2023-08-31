using System.Collections.Generic;

namespace ClassifiedAds.Infrastructure.MessageBrokers.RabbitMQ;

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
}
