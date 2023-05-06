using ClassifiedAds.Infrastructure.Monitoring.AzureApplicationInsights;
using ClassifiedAds.Infrastructure.Monitoring.MiniProfiler;
using ClassifiedAds.Infrastructure.Monitoring.OpenTelemetry;

namespace ClassifiedAds.Infrastructure.Monitoring;

public class MonitoringOptions
{
    public MiniProfilerOptions MiniProfiler { get; set; }

    public AzureApplicationInsightsOptions AzureApplicationInsights { get; set; }

    public OpenTelemetryOptions OpenTelemetry { get; set; }
}
