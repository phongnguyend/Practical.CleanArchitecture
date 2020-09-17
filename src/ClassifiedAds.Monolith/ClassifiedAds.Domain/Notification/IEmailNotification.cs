using ClassifiedAds.Domain.Entities;

namespace ClassifiedAds.Domain.Notification
{
    public interface IEmailNotification
    {
        void Send(EmailMessage emailMessage);
    }
}
