using ClassifiedAds.Domain.Entities;
using ClassifiedAds.DomainServices.Repositories;

namespace ClassifiedAds.DomainServices.Services
{
    public class SmsMessageService : GenericService<SmsMessage>, ISmsMessageService
    {
        public SmsMessageService(IUnitOfWork unitOfWork, IRepository<SmsMessage> repository) : base(unitOfWork, repository)
        {

        }
    }
}
