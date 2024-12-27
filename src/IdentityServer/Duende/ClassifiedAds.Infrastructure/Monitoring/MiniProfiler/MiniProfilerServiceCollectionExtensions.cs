using Microsoft.Extensions.DependencyInjection;
using StackExchange.Profiling.Storage;
using System.Security.Claims;

namespace ClassifiedAds.Infrastructure.Monitoring.MiniProfiler;

public static class MiniProfilerServiceCollectionExtensions
{
    public static IServiceCollection AddMiniProfiler(this IServiceCollection services, MiniProfilerOptions miniProfilerOptions = null)
    {
        if (miniProfilerOptions?.IsEnabled ?? false)
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
                if (!string.IsNullOrEmpty(miniProfilerOptions?.SqlServerStorage?.ConectionString))
                {
                    var storageOpt = miniProfilerOptions.SqlServerStorage;
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

        return services;
    }
}
