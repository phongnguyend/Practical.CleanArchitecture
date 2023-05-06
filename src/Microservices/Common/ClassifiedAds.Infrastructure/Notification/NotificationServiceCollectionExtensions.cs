using ClassifiedAds.Infrastructure.Notification;

namespace Microsoft.Extensions.DependencyInjection;

public static class NotificationServiceCollectionExtensions
{
    public static IServiceCollection AddNotificationServices(this IServiceCollection services, NotificationOptions options)
    {
        services.AddEmailNotification(options.Email);

        services.AddSmsNotification(options.Sms);

        return services;
    }
}
