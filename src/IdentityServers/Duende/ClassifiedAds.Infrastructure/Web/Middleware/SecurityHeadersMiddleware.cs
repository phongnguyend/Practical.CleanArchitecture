using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Web.Middleware;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly Dictionary<string, string> _headers;

    public SecurityHeadersMiddleware(RequestDelegate next, Dictionary<string, string> headers)
    {
        _next = next;
        _headers = headers;
    }

    public async Task Invoke(HttpContext context)
    {
        foreach (var header in _headers)
        {
            context.Response.Headers[header.Key] = header.Value;
        }

        await _next(context);
    }
}
