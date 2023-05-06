using System.Collections.Generic;

namespace ClassifiedAds.Infrastructure.MessageBrokers.AzureEventGrid;

public class AzureEventGridOptions
{
    public string DomainEndpoint { get; set; }

    public string DomainKey { get; set; }

    public Dictionary<string, string> Topics { get; set; }
}
