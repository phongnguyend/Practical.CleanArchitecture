using ClassifiedAds.Infrastructure.Monitoring.AzureApplicationInsights;
using ClassifiedAds.Infrastructure.Monitoring.MiniProfiler;
using ClassifiedAds.Infrastructure.Monitoring.OpenTelemetry;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ClassifiedAds.Infrastructure.Monitoring;

public static class MonitoringExtensions
{
    public static IServiceCollection AddMonitoringServices(this IServiceCollection services, MonitoringOptions monitoringOptions = null)
    {
        if (monitoringOptions?.MiniProfiler?.IsEnabled ?? false)
        {
            services.AddMiniProfiler(monitoringOptions.MiniProfiler);
        }

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

    public static IApplicationBuilder UseMonitoringServices(this IApplicationBuilder builder, MonitoringOptions monitoringOptions)
    {
        if (monitoringOptions?.MiniProfiler?.IsEnabled ?? false)
        {
            builder.UseMiniProfiler();
        }

        return builder;
    }
}
