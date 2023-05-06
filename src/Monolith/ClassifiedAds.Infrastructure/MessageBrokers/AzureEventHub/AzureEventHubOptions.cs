using System.Collections.Generic;

namespace ClassifiedAds.Infrastructure.MessageBrokers.AzureEventHub;

public class AzureEventHubOptions
{
    public string ConnectionString { get; set; }

    public string StorageConnectionString { get; set; }

    public Dictionary<string, string> Hubs { get; set; }

    public Dictionary<string, string> StorageContainerNames { get; set; }
}
