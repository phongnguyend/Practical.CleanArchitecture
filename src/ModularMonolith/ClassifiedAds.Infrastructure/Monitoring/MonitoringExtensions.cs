using App.Metrics;
using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Prometheus;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MonitoringExtensions
    {
        public static IWebHostBuilder UseClassifiedAdsMonitoringServices(this IWebHostBuilder hostBuilder)
        {
            hostBuilder.UseMetrics((context, options) =>
            {
                var metrics = AppMetrics.CreateDefaultBuilder()
                .Configuration.Configure(metricsOptions =>
                {
                    context.Configuration.GetSection("AppMetrics:MetricsOptions").Bind(metricsOptions);
                })
                .OutputMetrics.AsPrometheusPlainText()
                .Build();

                options.EndpointOptions = endpointsOptions =>
                {
                    context.Configuration.GetSection("AppMetrics:MetricEndpointsOptions").Bind(endpointsOptions);
                    endpointsOptions.MetricsTextEndpointOutputFormatter = metrics.OutputMetricsFormatters.OfType<MetricsPrometheusTextOutputFormatter>().First();
                    endpointsOptions.MetricsEndpointOutputFormatter = metrics.OutputMetricsFormatters.OfType<MetricsPrometheusTextOutputFormatter>().First();
                };

                options.TrackingMiddlewareOptions = trackingMiddlewareOptions =>
                {
                    context.Configuration.GetSection("AppMetrics:MetricsWebTrackingOptions").Bind(trackingMiddlewareOptions);
                };
            });

            return hostBuilder;
        }

        public static IMvcBuilder AddClassifiedAdsMonitoringServices(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder.AddMetrics();

            return mvcBuilder;
        }
    }
}
