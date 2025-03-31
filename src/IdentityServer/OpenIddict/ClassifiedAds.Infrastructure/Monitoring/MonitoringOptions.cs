using ClassifiedAds.Infrastructure.Monitoring.AzureApplicationInsights;

namespace ClassifiedAds.Infrastructure.Monitoring;

public class MonitoringOptions
{
    public AzureApplicationInsightsOptions AzureApplicationInsights { get; set; }
}
