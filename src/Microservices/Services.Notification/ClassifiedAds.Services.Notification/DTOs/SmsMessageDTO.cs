using ClassifiedAds.Infrastructure.Notification.Sms;

namespace ClassifiedAds.Services.Notification.DTOs;

public class SmsMessageDTO : ISmsMessage
{
    public string Message { get; set; }

    public string PhoneNumber { get; set; }
}
