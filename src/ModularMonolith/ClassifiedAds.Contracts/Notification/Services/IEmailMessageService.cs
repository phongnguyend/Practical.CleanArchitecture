using ClassifiedAds.Contracts.Notification.DTOs;

namespace ClassifiedAds.Contracts.Notification.Services;

public interface IEmailMessageService
{
    Task CreateEmailMessageAsync(EmailMessageDTO emailMessage);
}
