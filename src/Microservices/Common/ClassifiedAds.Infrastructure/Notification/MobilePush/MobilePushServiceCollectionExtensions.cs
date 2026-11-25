using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Claims;
using System.Threading.RateLimiting;

namespace ClassifiedAds.Infrastructure.Notification.MobilePush;

public static class MobilePushServiceCollectionExtensions
{
    public static IServiceCollection AddMobilePushNotifications<TContext>(this IServiceCollection services, IConfiguration configuration)
        where TContext : DbContext
    {
        var section = configuration;
        var options = section.Get<MobilePushOptions>() ?? new MobilePushOptions();
        if (options.UsedFirebase() && string.IsNullOrWhiteSpace(options.Firebase.ProjectId))
        {
            throw new InvalidOperationException("Enabled push notifications require Firebase ProjectId.");
        }
        services.Configure<MobilePushOptions>(section);
        services.AddScoped(provider => new MobilePushStore(provider.GetRequiredService<TContext>()));
        services.AddScoped<IMobilePushNotificationService, MobilePushNotificationService>();
        if (options.UsedFake())
        {
            services.AddSingleton<IMobilePushSender, FakeMobilePushSender>();
        }
        else if (options.UsedFirebase())
        {
            services.AddSingleton<IMobilePushSender, FirebaseMobilePushSender>();
        }
        else
        {
            throw new InvalidOperationException("Unsupported push notification provider.");
        }
        services.AddHostedService<MobilePushDeliveryWorker>();
        services.AddRateLimiter(limiter => limiter.AddPolicy("PushNotifications", context => RateLimitPartition.GetFixedWindowLimiter(
            context.User.FindFirstValue("sub") ?? context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            _ => new FixedWindowRateLimiterOptions { PermitLimit = 60, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 })));
        return services;
    }
}
