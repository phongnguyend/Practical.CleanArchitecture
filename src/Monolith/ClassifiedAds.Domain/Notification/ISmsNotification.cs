using ClassifiedAds.Domain.Entities;

namespace ClassifiedAds.Domain.Notification
{
    public interface ISmsNotification
    {
        void Send(SmsMessage smsMessage);
    }
}
