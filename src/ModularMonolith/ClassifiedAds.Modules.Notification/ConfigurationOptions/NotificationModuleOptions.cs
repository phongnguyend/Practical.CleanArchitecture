using ClassifiedAds.Infrastructure.MessageBrokers;
using ClassifiedAds.Infrastructure.Notification;

namespace ClassifiedAds.Modules.Notification.ConfigurationOptions
{
    public class NotificationModuleOptions
    {
        public ConnectionStringsOptions ConnectionStrings { get; set; }

        public MessageBrokerOptions MessageBroker { get; set; }

        public NotificationOptions Notification { get; set; }
    }
}
