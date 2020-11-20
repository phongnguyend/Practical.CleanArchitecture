using App.Metrics;
using App.Metrics.AspNetCore;
using App.Metrics.AspNetCore.Endpoints;
using App.Metrics.AspNetCore.Tracking;
using App.Metrics.Formatters.Prometheus;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace ClassifiedAds.Infrastructure.Monitoring.AppMetrics
{
    public static class AppMetricsServiceCollectionExtensions
    {
        public static IServiceCollection AddAppMetrics(this IServiceCollection services, AppMetricsOptions appMetricsOptions = null)
        {
            if (appMetricsOptions?.IsEnabled ?? false)
            {
                var metrics = App.Metrics.AppMetrics.CreateDefaultBuilder()
                .Configuration.Configure(appMetricsOptions.MetricsOptions)
                .OutputMetrics.AsPrometheusPlainText()
                .Build();

                var options = new MetricsWebHostOptions
                {
                    EndpointOptions = endpointsOptions =>
                    {
                        endpointsOptions.MetricsEndpointEnabled = appMetricsOptions.MetricEndpointsOptions.MetricsEndpointEnabled;
                        endpointsOptions.MetricsTextEndpointEnabled = appMetricsOptions.MetricEndpointsOptions.MetricsTextEndpointEnabled;
                        endpointsOptions.EnvironmentInfoEndpointEnabled = appMetricsOptions.MetricEndpointsOptions.EnvironmentInfoEndpointEnabled;
                        endpointsOptions.MetricsTextEndpointOutputFormatter = metrics.OutputMetricsFormatters.OfType<MetricsPrometheusTextOutputFormatter>().First();
                        endpointsOptions.MetricsEndpointOutputFormatter = metrics.OutputMetricsFormatters.OfType<MetricsPrometheusTextOutputFormatter>().First();
                    },

                    TrackingMiddlewareOptions = trackingMiddlewareOptions =>
                    {
                        trackingMiddlewareOptions.ApdexTrackingEnabled = appMetricsOptions.MetricsWebTrackingOptions.ApdexTrackingEnabled;
                        trackingMiddlewareOptions.ApdexTSeconds = appMetricsOptions.MetricsWebTrackingOptions.ApdexTSeconds;
                        trackingMiddlewareOptions.IgnoredHttpStatusCodes = appMetricsOptions.MetricsWebTrackingOptions.IgnoredHttpStatusCodes;
                        trackingMiddlewareOptions.IgnoredRoutesRegexPatterns = appMetricsOptions.MetricsWebTrackingOptions.IgnoredRoutesRegexPatterns;
                        trackingMiddlewareOptions.OAuth2TrackingEnabled = appMetricsOptions.MetricsWebTrackingOptions.OAuth2TrackingEnabled;
                    },
                };

                services.AddMetrics(metrics);

                services.AddMetricsReportingHostedService(options.UnobservedTaskExceptionHandler);
                services.AddMetricsEndpoints(options.EndpointOptions);
                services.AddMetricsTrackingMiddleware(options.TrackingMiddlewareOptions);

                services.AddSingleton<IStartupFilter>(new DefaultMetricsEndpointsStartupFilter());
                services.AddSingleton<IStartupFilter>(new DefaultMetricsTrackingStartupFilter());
            }

            return services;
        }
    }
}
