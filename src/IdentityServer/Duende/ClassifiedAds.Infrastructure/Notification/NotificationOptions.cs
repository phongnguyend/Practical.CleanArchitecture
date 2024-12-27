using ClassifiedAds.Infrastructure.Notification.Email;
using ClassifiedAds.Infrastructure.Notification.Sms;
using ClassifiedAds.Infrastructure.Notification.Web;

namespace ClassifiedAds.Infrastructure.Notification;

public class NotificationOptions
{
    public EmailOptions Email { get; set; }

    public SmsOptions Sms { get; set; }

    public WebOptions Web { get; set; }
}
