using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ClassifiedAds.WebMVC.Middleware
{
    public class IPFilteringMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<IPFilteringMiddleware> _logger;

        public IPFilteringMiddleware(RequestDelegate next, ILogger<IPFilteringMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var remoteIp = context.Connection.RemoteIpAddress;
            _logger.LogInformation($"Request from Remote IP address: {remoteIp}");

            await _next(context);
        }
    }
}
