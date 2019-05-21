using ClassifiedAds.Domain.Entities;
using ClassifiedAds.DomainServices.Repositories;

namespace ClassifiedAds.DomainServices.Services
{
    public class EmailMessageService : IEmailMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<EmailMessage> _repository;

        public EmailMessageService(IUnitOfWork unitOfWork, IRepository<EmailMessage> repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public void Add(EmailMessage email)
        {
            _repository.Add(email);
            _unitOfWork.SaveChanges();
        }
    }
}
