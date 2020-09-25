using ClassifiedAds.Infrastructure.Profiling;
using ClassifiedAds.Infrastructure.Profiling.AzureApplicationInsights;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using StackExchange.Profiling.Storage;
using System.Security.Claims;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ProfilingExtensions
    {
        public static IServiceCollection AddClassifiedAdsProfiler(this IServiceCollection services, MonitoringOptions monitoringOptions = null)
        {
            if (monitoringOptions?.MiniProfiler?.IsEnabled ?? false)
            {
                services.AddMiniProfiler(options =>
                {
                    options.UserIdProvider = (request) =>
                    {
                        var id = request.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                ?? request.HttpContext.User.FindFirst("sub")?.Value;
                        return id;
                    };

                    options.RouteBasePath = "/profiler"; // access /profiler/results to see last profile check
                    options.PopupRenderPosition = StackExchange.Profiling.RenderPosition.BottomLeft;
                    options.PopupShowTimeWithChildren = true;
                    if (!string.IsNullOrEmpty(monitoringOptions.MiniProfiler?.SqlServerStorage?.ConectionString))
                    {
                        var storageOpt = monitoringOptions.MiniProfiler.SqlServerStorage;
                        var storage = new SqlServerStorage(storageOpt.ConectionString, storageOpt.ProfilersTable, storageOpt.TimingsTable, storageOpt.ClientTimingsTable);
                        _ = storage.TableCreationScripts;

                        options.Storage = storage;
                    }

                    options.ShouldProfile = (request) =>
                    {
                        if (request.Path.StartsWithSegments("/healthcheck"))
                        {
                            return false;
                        }

                        return true;
                    };
                })
                .AddEntityFramework();
            }

            if (monitoringOptions?.AzureApplicationInsights?.IsEnabled ?? false)
            {
                services.AddApplicationInsightsTelemetry(opt =>
                {
                    opt.InstrumentationKey = monitoringOptions.AzureApplicationInsights.InstrumentationKey;
                });

                services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) =>
                {
                    module.EnableSqlCommandTextInstrumentation = monitoringOptions.AzureApplicationInsights.EnableSqlCommandTextInstrumentation;
                });

                services.AddApplicationInsightsTelemetryProcessor<CustomTelemetryProcessor>();
                services.AddSingleton<ITelemetryInitializer, CustomTelemetryInitializer>();
            }

            return services;
        }

        public static IApplicationBuilder UseClassifiedAdsProfiler(this IApplicationBuilder builder)
        {
            builder.UseMiniProfiler();
            return builder;
        }
    }
}
