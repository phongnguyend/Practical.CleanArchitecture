using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ClassifiedAds.WebMVC.Middleware
{
    public class CustomMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            await _next(context);
            var elapsedTime = stopwatch.Elapsed;

            if (context.Request.Path.HasValue && context.Request.Path.Value.Contains("oidc"))
            {
            }
        }
    }
}
