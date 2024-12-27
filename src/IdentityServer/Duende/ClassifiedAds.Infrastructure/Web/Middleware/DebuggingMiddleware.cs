using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Web.Middleware;

public class DebuggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DebuggingMiddleware> _logger;

    public DebuggingMiddleware(RequestDelegate next, ILogger<DebuggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
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
