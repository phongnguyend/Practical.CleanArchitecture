using ClassifiedAds.Infrastructure.Monitoring.AzureApplicationInsights;
using ClassifiedAds.Infrastructure.Monitoring.OpenTelemetry;

namespace ClassifiedAds.Infrastructure.Monitoring;

public class MonitoringOptions
{
    public AzureApplicationInsightsOptions AzureApplicationInsights { get; set; }

    public OpenTelemetryOptions OpenTelemetry { get; set; }
}
