using ClassifiedAds.Domain.Notification;
using ClassifiedAds.Infrastructure.Notification.Email;
using ClassifiedAds.Infrastructure.Notification.Email.SmtpClient;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EmailNotificationServiceCollectionExtensions
    {
        public static IServiceCollection AddSmtpClientEmailNotification(this IServiceCollection services, SmtpClientOptions options)
        {
            services.AddSingleton<IEmailNotification>(new SmtpClientEmailNotification(options));
            return services;
        }

        public static IServiceCollection AddFakeEmailNotification(this IServiceCollection services)
        {
            services.AddSingleton<IEmailNotification>(new FakeEmailNotification());
            return services;
        }

        public static IServiceCollection AddEmailNotification(this IServiceCollection services, EmailOptions options)
        {
            if (options.UsedFake())
            {
                services.AddFakeEmailNotification();
            }
            else if (options.UsedSmtpClient())
            {
                services.AddSmtpClientEmailNotification(options.SmtpClient);
            }

            return services;
        }
    }
}
