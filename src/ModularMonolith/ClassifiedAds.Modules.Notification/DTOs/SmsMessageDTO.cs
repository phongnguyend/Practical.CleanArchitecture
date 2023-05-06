using ClassifiedAds.Infrastructure.Notification.Sms;

namespace ClassifiedAds.Modules.Notification.DTOs;

public class SmsMessageDTO : ISmsMessage
{
    public string Message { get; set; }

    public string PhoneNumber { get; set; }
}
