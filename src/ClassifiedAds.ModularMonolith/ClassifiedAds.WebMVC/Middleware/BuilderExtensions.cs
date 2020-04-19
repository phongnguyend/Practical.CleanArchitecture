using ClassifiedAds.WebMVC.Middleware;

namespace Microsoft.AspNetCore.Builder
{
    public static class BuilderExtensions
    {
        public static IApplicationBuilder UseIPFiltering(this IApplicationBuilder app)
        {
            app.UseMiddleware<IPFilteringMiddleware>();
            return app;
        }
    }
}
