using ClassifiedAds.Application;
using ClassifiedAds.Contracts.Notification.DTOs;
using ClassifiedAds.Contracts.Notification.Services;
using ClassifiedAds.Modules.Notification.Entities;
using ClassifiedAds.Modules.Notification.Repositories;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.Notification.Services;

public class EmailMessageService : CrudService<EmailMessage>, IEmailMessageService
{
    public EmailMessageService(IEmailMessageRepository repository, Dispatcher dispatcher)
        : base(repository, dispatcher)
    {
    }

    public Task CreateEmailMessageAsync(EmailMessageDTO emailMessage)
    {
        return AddOrUpdateAsync(new EmailMessage
        {
            From = emailMessage.From,
            Tos = emailMessage.Tos,
            CCs = emailMessage.CCs,
            BCCs = emailMessage.BCCs,
            Subject = emailMessage.Subject,
            Body = emailMessage.Body,
        });
    }
}
