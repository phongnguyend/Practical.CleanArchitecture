using App.Metrics;
using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Prometheus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MonitoringServicesExtensions
    {
        public static IWebHostBuilder UseMonitoringServices(this IWebHostBuilder hostBuilder)
        {
            var metrics = AppMetrics.CreateDefaultBuilder()
                  .OutputMetrics.AsPrometheusPlainText()
                  .Build();

            hostBuilder.UseMetrics(options =>
            {
                options.EndpointOptions = endpointsOptions =>
                {
                    endpointsOptions.MetricsTextEndpointOutputFormatter = metrics.OutputMetricsFormatters.OfType<MetricsPrometheusTextOutputFormatter>().First();
                    endpointsOptions.MetricsEndpointOutputFormatter = metrics.OutputMetricsFormatters.OfType<MetricsPrometheusTextOutputFormatter>().First();
                };
            })
                .UseMetricsWebTracking();

            return hostBuilder;
        }

        public static IServiceCollection AddMonitoringServices(this IServiceCollection services)
        {
            // If using Kestrel:
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            // If using IIS:
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.AddMetricsTrackingMiddleware();

            return services;
        }

        public static IMvcBuilder AddMonitoringServices(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder.AddMetrics();

            return mvcBuilder;
        }

        public static IApplicationBuilder UseMonitoringServices(this IApplicationBuilder app)
        {
            app.UseMetricsAllMiddleware();

            return app;
        }
    }
}
