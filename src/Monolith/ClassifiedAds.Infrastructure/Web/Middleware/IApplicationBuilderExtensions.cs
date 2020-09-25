using ClassifiedAds.Infrastructure.Web.Middleware;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Builder
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseIPFiltering(this IApplicationBuilder app)
        {
            app.UseMiddleware<IPFilteringMiddleware>();
            return app;
        }

        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app, Dictionary<string, string> headers)
        {
            app.UseMiddleware<SecurityHeadersMiddleware>(headers);
            return app;
        }

        public static IApplicationBuilder UseDebuggingMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<DebuggingMiddleware>();
            return app;
        }
    }
}
