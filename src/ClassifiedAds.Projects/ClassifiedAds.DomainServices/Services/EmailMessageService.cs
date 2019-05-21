using ClassifiedAds.Domain.Entities;
using ClassifiedAds.DomainServices.Repositories;

namespace ClassifiedAds.DomainServices.Services
{
    public class EmailMessageService : GenericService<EmailMessage>, IEmailMessageService
    {
        public EmailMessageService(IUnitOfWork unitOfWork, IRepository<EmailMessage> repository)
            : base(unitOfWork, repository)
        {

        }
    }
}
