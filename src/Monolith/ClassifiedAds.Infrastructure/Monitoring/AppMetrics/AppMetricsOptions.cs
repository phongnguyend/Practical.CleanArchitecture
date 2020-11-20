using App.Metrics;
using App.Metrics.AspNetCore.Endpoints;
using App.Metrics.AspNetCore.Tracking;

namespace ClassifiedAds.Infrastructure.Monitoring.AppMetrics
{
    public class AppMetricsOptions
    {
        public bool IsEnabled { get; set; }

        public MetricsOptions MetricsOptions { get; set; }

        public MetricsWebTrackingOptions MetricsWebTrackingOptions { get; set; }

        public MetricEndpointsOptions MetricEndpointsOptions { get; set; }
    }
}
