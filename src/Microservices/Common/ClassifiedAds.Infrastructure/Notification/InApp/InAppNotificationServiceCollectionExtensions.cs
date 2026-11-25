using ClassifiedAds.Infrastructure.Notification.InApp;
using ClassifiedAds.Infrastructure.Notification.InApp.Fake;
using ClassifiedAds.Infrastructure.Notification.InApp.SignalR;

namespace Microsoft.Extensions.DependencyInjection;

public static class InAppNotificationServiceCollectionExtensions
{
    public static IServiceCollection AddSignalRInAppNotification<T>(this IServiceCollection services, SignalROptions options)
    {
        services.AddSingleton<IInAppNotification<T>>(new SignalRInAppNotification<T>(options.Endpoint, options.Hubs[typeof(T).Name], options.MethodNames[typeof(T).Name]));
        return services;
    }

    public static IServiceCollection AddFakeInAppNotification<T>(this IServiceCollection services)
    {
        services.AddSingleton<IInAppNotification<T>>(new FakeInAppNotification<T>());
        return services;
    }

    public static IServiceCollection AddInAppNotification<T>(this IServiceCollection services, InAppOptions options)
    {
        if (options.UsedFake())
        {
            services.AddFakeInAppNotification<T>();
        }
        else if (options.UsedSignalR())
        {
            services.AddSignalRInAppNotification<T>(options.SignalR);
        }

        return services;
    }
}
