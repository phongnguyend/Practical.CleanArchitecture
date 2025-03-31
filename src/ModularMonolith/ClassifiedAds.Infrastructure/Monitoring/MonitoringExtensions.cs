using ClassifiedAds.Infrastructure.Monitoring.AzureApplicationInsights;
using ClassifiedAds.Infrastructure.Monitoring.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;

namespace ClassifiedAds.Infrastructure.Monitoring;

public static class MonitoringExtensions
{
    public static IServiceCollection AddMonitoringServices(this IServiceCollection services, MonitoringOptions monitoringOptions = null)
    {
        if (monitoringOptions?.AzureApplicationInsights?.IsEnabled ?? false)
        {
            services.AddAzureApplicationInsights(monitoringOptions.AzureApplicationInsights);
        }

        if (monitoringOptions?.OpenTelemetry?.IsEnabled ?? false)
        {
            services.AddClassifiedAdsOpenTelemetry(monitoringOptions.OpenTelemetry);
        }

        return services;
    }
}
