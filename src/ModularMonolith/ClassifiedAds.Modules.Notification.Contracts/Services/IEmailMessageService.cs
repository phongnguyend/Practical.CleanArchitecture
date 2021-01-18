using ClassifiedAds.Modules.Notification.Contracts.DTOs;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.Notification.Contracts.Services
{
    public interface IEmailMessageService
    {
        Task CreateEmailMessageAsync(EmailMessageDTO emailMessage);
    }
}
