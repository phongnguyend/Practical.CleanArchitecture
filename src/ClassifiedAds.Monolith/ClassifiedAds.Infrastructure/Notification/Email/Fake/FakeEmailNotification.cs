using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Notification;

namespace ClassifiedAds.Infrastructure.Notification.Email.SmtpClient
{
    public class FakeEmailNotification : IEmailNotification
    {
        public void Send(EmailMessage emailMessage)
        {
            // do nothing
        }
    }
}
