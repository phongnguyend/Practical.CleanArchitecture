using System.Collections.Generic;

namespace ClassifiedAds.Infrastructure.Messaging.AzureServiceBus;

public class AzureServiceBusOptions
{
    public string ConnectionString { get; set; }

    public Dictionary<string, string> QueueNames { get; set; }
}
