using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Web.Middleware;

public class LoggingStatusCodeMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingStatusCodeMiddleware> _logger;

    public LoggingStatusCodeMiddleware(RequestDelegate next, ILogger<LoggingStatusCodeMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        var statusCode = context.Response.StatusCode;
        var path = context.Request.Path;
        var method = context.Request.Method;
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? context.User.FindFirst("sub")?.Value;
        var remoteIp = context.Connection.RemoteIpAddress;

        var statusCodes = new[] { StatusCodes.Status401Unauthorized, StatusCodes.Status403Forbidden };

        if (statusCodes.Contains(statusCode))
        {
            _logger.LogWarning($"StatusCode: {statusCode}, UserId: {ReplaceCRLF(userId)}, Path: {ReplaceCRLF(path)}, Method: {ReplaceCRLF(method)}, IP: {remoteIp}");
        }
    }

    private static string ReplaceCRLF(string text)
    {
        return text?.Replace("\r", "\\r").Replace("\n", "\\n");
    }
}
