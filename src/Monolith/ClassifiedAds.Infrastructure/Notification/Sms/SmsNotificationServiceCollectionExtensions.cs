using ClassifiedAds.Domain.Notification;
using ClassifiedAds.Infrastructure.Notification.Sms;
using ClassifiedAds.Infrastructure.Notification.Sms.Azure;
using ClassifiedAds.Infrastructure.Notification.Sms.Fake;
using ClassifiedAds.Infrastructure.Notification.Sms.Twilio;

namespace Microsoft.Extensions.DependencyInjection;

public static class SmsNotificationServiceCollectionExtensions
{
    public static IServiceCollection AddTwilioSmsNotification(this IServiceCollection services, TwilioOptions options)
    {
        services.AddSingleton<ISmsNotification>(new TwilioSmsNotification(options));
        return services;
    }

    public static IServiceCollection AddAzureSmsNotification(this IServiceCollection services, AzureOptions options)
    {
        services.AddSingleton<ISmsNotification>(new AzureSmsNotification(options));
        return services;
    }

    public static IServiceCollection AddFakeSmsNotification(this IServiceCollection services)
    {
        services.AddSingleton<ISmsNotification>(new FakeSmsNotification());
        return services;
    }

    public static IServiceCollection AddSmsNotification(this IServiceCollection services, SmsOptions options)
    {
        if (options.UsedFake())
        {
            services.AddFakeSmsNotification();
        }
        else if (options.UsedTwilio())
        {
            services.AddTwilioSmsNotification(options.Twilio);
        }
        else if (options.UsedAzure())
        {
            services.AddAzureSmsNotification(options.Azure);
        }

        return services;
    }
}
