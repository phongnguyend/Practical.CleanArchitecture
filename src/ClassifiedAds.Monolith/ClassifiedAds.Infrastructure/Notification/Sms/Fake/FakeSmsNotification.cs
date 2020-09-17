using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Notification;

namespace ClassifiedAds.Infrastructure.Notification.Sms.Fake
{
    public class FakeSmsNotification : ISmsNotification
    {
        public void Send(SmsMessage smsMessage)
        {
            // do nothing
        }
    }
}
