using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Reflection;

namespace ClassifiedAds.Infrastructure.Monitoring.OpenTelemetry;

public static class OpenTelemetryExtensions
{
    public static IServiceCollection AddClassifiedAdsOpenTelemetry(this IServiceCollection services, OpenTelemetryOptions options = null)
    {
        if (options == null || !options.IsEnabled)
        {
            return services;
        }

        if (!options.TraceEnabled && !options.MetricEnabled)
        {
            return services;
        }

        var openTelemetry = services.AddOpenTelemetry()
              .ConfigureResource(configureResource =>
              {
                  configureResource.AddService(
                      serviceName: options.ServiceName,
                      serviceVersion: Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown",
                      serviceInstanceId: options.ServiceName + "-" + Environment.MachineName);
              });

        if (options.TraceEnabled)
        {
            openTelemetry.WithTracing(builder =>
            {
                builder
                .SetSampler(new AlwaysOnSampler())
                .AddAspNetCoreInstrumentation()
                .AddEntityFrameworkCoreInstrumentation()
                .AddHttpClientInstrumentation();

                if (options?.Zipkin?.IsEnabled ?? false)
                {
                    builder.AddZipkinExporter(zipkinOptions =>
                    {
                        zipkinOptions.Endpoint = new Uri(options.Zipkin.Endpoint);
                    });
                }

                if (options?.Otlp?.IsEnabled ?? false)
                {
                    builder.AddOtlpExporter(otlpOptions =>
                    {
                        otlpOptions.Endpoint = new Uri(options.Otlp.Endpoint);
                    });
                }

                if (options?.AzureMonitor?.IsEnabled ?? false)
                {
                    builder.AddAzureMonitorTraceExporter(opts =>
                    {
                        opts.ConnectionString = options.AzureMonitor.ConnectionString;
                    });
                }
            });
        }

        if (options.MetricEnabled)
        {
            openTelemetry.WithMetrics(builder =>
            {
                builder
                .AddRuntimeInstrumentation()
                .AddProcessInstrumentation()
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation();

                if (options?.Otlp?.IsEnabled ?? false)
                {
                    builder.AddOtlpExporter(otlpOptions =>
                    {
                        otlpOptions.Endpoint = new Uri(options.Otlp.Endpoint);
                    });
                }

                if (options?.AzureMonitor?.IsEnabled ?? false)
                {
                    builder.AddAzureMonitorMetricExporter(opts =>
                    {
                        opts.ConnectionString = options.AzureMonitor.ConnectionString;
                    });
                }
            });
        }

        return services;
    }
}
