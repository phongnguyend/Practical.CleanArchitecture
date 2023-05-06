using ClassifiedAds.Domain.Notification;
using ClassifiedAds.Infrastructure.Notification.Web;
using ClassifiedAds.Infrastructure.Notification.Web.Fake;
using ClassifiedAds.Infrastructure.Notification.Web.SignalR;

namespace Microsoft.Extensions.DependencyInjection;

public static class WebNotificationServiceCollectionExtensions
{
    public static IServiceCollection AddSignalRWebNotification<T>(this IServiceCollection services, SignalROptions options)
    {
        services.AddSingleton<IWebNotification<T>>(new SignalRNotification<T>(options.Endpoint, options.Hubs[typeof(T).Name], options.MethodNames[typeof(T).Name]));
        return services;
    }

    public static IServiceCollection AddFakeWebNotification<T>(this IServiceCollection services)
    {
        services.AddSingleton<IWebNotification<T>>(new FakeWebNotification<T>());
        return services;
    }

    public static IServiceCollection AddWebNotification<T>(this IServiceCollection services, WebOptions options)
    {
        if (options.UsedFake())
        {
            services.AddFakeWebNotification<T>();
        }
        else if (options.UsedSignalR())
        {
            services.AddSignalRWebNotification<T>(options.SignalR);
        }

        return services;
    }
}
