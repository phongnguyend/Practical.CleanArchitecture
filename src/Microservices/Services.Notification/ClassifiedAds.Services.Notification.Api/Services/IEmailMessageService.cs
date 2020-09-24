using ClassifiedAds.Services.Notification.DTOs;

namespace ClassifiedAds.Services.Notification.Services
{
    public interface IEmailMessageService
    {
        void CreateEmailMessage(EmailMessageDTO emailMessage);
    }
}
