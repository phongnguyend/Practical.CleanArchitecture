using ClassifiedAds.Infrastructure.Notification.Email;
using ClassifiedAds.Infrastructure.Notification.MobilePush;
using ClassifiedAds.Infrastructure.Notification.Sms;
using ClassifiedAds.Infrastructure.Notification.InApp;

namespace ClassifiedAds.Infrastructure.Notification;

public class NotificationOptions
{
    public EmailOptions Email { get; set; }

    public SmsOptions Sms { get; set; }

    public InAppOptions InApp { get; set; }

    public MobilePushOptions MobilePush { get; set; } = new();
}
