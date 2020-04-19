using Microsoft.AspNetCore.Builder;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ProfilingExtensions
    {
        public static IServiceCollection AddClassifiedAdsProfiler(this IServiceCollection services)
        {
            services.AddMemoryCache()
            .AddMiniProfiler(options =>
            {
                options.RouteBasePath = "/profiler"; // access /profiler/results to see last profile check
                options.PopupRenderPosition = StackExchange.Profiling.RenderPosition.BottomLeft;
                options.PopupShowTimeWithChildren = true;
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
