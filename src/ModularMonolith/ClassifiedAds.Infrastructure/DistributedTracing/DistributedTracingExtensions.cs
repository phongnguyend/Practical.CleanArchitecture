using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;
using System;

namespace ClassifiedAds.Infrastructure.DistributedTracing
{
    public static class DistributedTracingExtensions
    {
        public static IServiceCollection AddDistributedTracing(this IServiceCollection services, DistributedTracingOptions options = null)
        {
            if (options == null || !options.IsEnabled)
            {
                return services;
            }

            if (options?.Exporter == ExporterOptions.Zipkin)
            {
                services.AddOpenTelemetryTracing((builder) => builder
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .SetSampler(new AlwaysOnSampler())
                        .AddZipkinExporter(zipkinOptions =>
                        {
                            zipkinOptions.ServiceName = options.Zipkin.ServiceName;
                            zipkinOptions.Endpoint = new Uri(options.Zipkin.Endpoint);
                        }));
            }

            if (options?.Exporter == ExporterOptions.Jaeger)
            {
                services.AddOpenTelemetryTracing((builder) => builder
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .SetSampler(new AlwaysOnSampler())
                        .AddJaegerExporter(jaegerOptions =>
                        {
                            jaegerOptions.AgentHost = options.Jaeger.Host;
                            jaegerOptions.AgentPort = options.Jaeger.Port;
                        }));
            }

            if (options?.Exporter == ExporterOptions.Otlp)
            {
                services.AddOpenTelemetryTracing((builder) => builder
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .SetSampler(new AlwaysOnSampler())
                        .AddOtlpExporter(otlpOptions =>
                        {
                            otlpOptions.Endpoint = options.Otlp.Endpoint;
                        }));
            }

            return services;
        }
    }
}
