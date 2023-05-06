using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using System;
using System.Threading;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace ClassifiedAds.WebAPI.RateLimiterPolicies;

public class GetAuditLogsRateLimiterPolicy : IRateLimiterPolicy<string>
{
    public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected { get; } = (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        return default;
    };

    public RateLimitPartition<string> GetPartition(HttpContext httpContext)
    {
        // same policy name and same partition key => will use the same rate limiter instance
        string partitionKey = null;

        if (httpContext.User.Identity?.IsAuthenticated == true)
        {
            partitionKey = httpContext.User.Identity.Name!;
            return RateLimitPartition.GetFixedWindowLimiter(partitionKey,
                partition => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 200,
                    Window = TimeSpan.FromMinutes(1),
                });
        }

        partitionKey = httpContext.Request.Headers.Host.ToString();
        return RateLimitPartition.GetFixedWindowLimiter(partitionKey,
            partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
            });
    }
}
