using ClassifiedAds.Modules.Notification.Contracts.DTOs;

namespace ClassifiedAds.Modules.Notification.Contracts.Services
{
    public interface IEmailMessageService
    {
        void CreateEmailMessage(EmailMessageDTO emailMessage);
    }
}
