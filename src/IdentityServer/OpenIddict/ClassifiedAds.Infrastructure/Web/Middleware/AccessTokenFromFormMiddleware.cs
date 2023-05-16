using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Web.Middleware;

internal class AccessTokenFromFormMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AccessTokenFromFormMiddleware> _logger;

    public AccessTokenFromFormMiddleware(RequestDelegate next, ILogger<AccessTokenFromFormMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Method == "POST"
            && context.Request.ContentType == "application/x-www-form-urlencoded"
            && string.IsNullOrEmpty(context.Request.Headers["Authorization"]))
        {
            var token = context.Request.Form["access_token"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Request.Headers.Add("Authorization", "Bearer " + token);
            }
        }

        await _next(context);
    }
}
