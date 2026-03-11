using Microsoft.Extensions.DependencyInjection;

namespace ClassifiedAds.Infrastructure.Monitoring.AzureApplicationInsights;

public static class AzureApplicationInsightsServiceCollectionExtensions
{
    public static IServiceCollection AddAzureApplicationInsights(this IServiceCollection services, AzureApplicationInsightsOptions azureApplicationInsightsOptions = null)
    {
        if (azureApplicationInsightsOptions?.IsEnabled ?? false)
        {
            services.AddApplicationInsightsTelemetry(opt =>
            {
                opt.ConnectionString = azureApplicationInsightsOptions.ConnectionString;
            });
        }

        return services;
    }
}
