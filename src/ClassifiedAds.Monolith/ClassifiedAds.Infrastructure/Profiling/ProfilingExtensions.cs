using Microsoft.AspNetCore.Builder;
using StackExchange.Profiling.Storage;
using System.Security.Claims;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ProfilingExtensions
    {
        public static IServiceCollection AddClassifiedAdsProfiler(this IServiceCollection services, string connectionString = "")
        {
            services.AddMemoryCache()
            .AddMiniProfiler(options =>
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
                if (!string.IsNullOrEmpty(connectionString))
                {
                    var storage = new SqlServerStorage(connectionString);
                    _ = storage.TableCreationScripts;

                    // options.Storage = storage;
                }
            })
            .AddEntityFramework();
            return services;
        }

        public static IApplicationBuilder UseClassifiedAdsProfiler(this IApplicationBuilder builder)
        {
            builder.UseMiniProfiler();
            return builder;
        }
    }
}
