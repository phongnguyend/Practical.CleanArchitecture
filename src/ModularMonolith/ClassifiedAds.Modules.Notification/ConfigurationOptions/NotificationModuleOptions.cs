using ClassifiedAds.Infrastructure.Notification;

namespace ClassifiedAds.Modules.Notification.ConfigurationOptions;

public class NotificationModuleOptions : NotificationOptions
{
    public ConnectionStringsOptions ConnectionStrings { get; set; }
}
