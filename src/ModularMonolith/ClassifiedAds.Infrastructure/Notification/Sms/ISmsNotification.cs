namespace ClassifiedAds.Infrastructure.Notification.Sms
{
    public interface ISmsNotification
    {
        void Send(SmsMessageDTO smsMessage);
    }

    public class SmsMessageDTO
    {
        public string Message { get; set; }

        public string PhoneNumber { get; set; }
    }
}
