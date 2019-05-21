using ClassifiedAds.Domain.Entities;

namespace ClassifiedAds.DomainServices.Services
{
    public interface IEmailMessageService
    {
        void Add(EmailMessage email);
    }
}
