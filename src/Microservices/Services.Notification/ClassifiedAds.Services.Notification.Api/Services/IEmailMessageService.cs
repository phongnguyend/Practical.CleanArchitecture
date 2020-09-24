using ClassifiedAds.Services.Notification.Contracts.DTOs;

namespace ClassifiedAds.Services.Notification.Contracts.Services
{
    public interface IEmailMessageService
    {
        void CreateEmailMessage(EmailMessageDTO emailMessage);
    }
}
