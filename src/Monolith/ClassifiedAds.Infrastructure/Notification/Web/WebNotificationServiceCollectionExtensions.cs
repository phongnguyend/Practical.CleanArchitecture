using ClassifiedAds.Domain.Notification;
using ClassifiedAds.Infrastructure.Notification.Web;
using ClassifiedAds.Infrastructure.Notification.Web.Fake;
using ClassifiedAds.Infrastructure.Notification.Web.SignalR;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class WebNotificationServiceCollectionExtensions
    {
        public static IServiceCollection AddSignalRWebNotification(this IServiceCollection services)
        {
            services.AddSingleton<IWebNotification>(new SignalRNotification());
            return services;
        }

        public static IServiceCollection AddFakeWebNotification(this IServiceCollection services)
        {
            services.AddSingleton<IWebNotification>(new FakeWebNotification());
            return services;
        }

        public static IServiceCollection AddWebNotification(this IServiceCollection services, WebOptions options)
        {
            if (options.UsedFake())
            {
                services.AddFakeWebNotification();
            }
            else if (options.UsedSignalR())
            {
                services.AddSignalRWebNotification();
            }

            return services;
        }
    }
}
