using System.Collections.Generic;

namespace ClassifiedAds.Infrastructure.MessageBrokers.AzureServiceBus;

public class AzureServiceBusOptions
{
    public string ConnectionString { get; set; }

    public Dictionary<string, string> QueueNames { get; set; }
}
