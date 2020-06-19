using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Web.Middleware
{
    public class DebuggingMiddleware
    {
        private readonly RequestDelegate _next;

        public DebuggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            await _next(context);
            var elapsedTime = stopwatch.Elapsed;

            // inspect the context here
            // if (context.Request.Path.HasValue && context.Request.Path.Value.Contains("oidc"))
            // {
            // }
        }
    }
}
