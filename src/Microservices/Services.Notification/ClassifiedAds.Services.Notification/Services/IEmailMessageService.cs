using ClassifiedAds.Services.Notification.DTOs;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Notification.Services
{
    public interface IEmailMessageService
    {
        Task CreateEmailMessageAsync(EmailMessageDTO emailMessage);
    }
}
