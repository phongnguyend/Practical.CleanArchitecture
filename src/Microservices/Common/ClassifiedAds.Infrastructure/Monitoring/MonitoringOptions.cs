using ClassifiedAds.Infrastructure.Monitoring.AppMetrics;
using ClassifiedAds.Infrastructure.Monitoring.AzureApplicationInsights;
using ClassifiedAds.Infrastructure.Monitoring.MiniProfiler;

namespace ClassifiedAds.Infrastructure.Monitoring
{
    public class MonitoringOptions
    {
        public MiniProfilerOptions MiniProfiler { get; set; }

        public AzureApplicationInsightsOptions AzureApplicationInsights { get; set; }

        public AppMetricsOptions AppMetrics { get; set; }
    }
}
