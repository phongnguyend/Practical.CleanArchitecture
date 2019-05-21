using ClassifiedAds.Domain.Entities;
using ClassifiedAds.DomainServices.Repositories;

namespace ClassifiedAds.DomainServices.Services
{
    public class SmsMessageService : ISmsMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<SmsMessage> _repository;

        public SmsMessageService(IUnitOfWork unitOfWork, IRepository<SmsMessage> repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }
        public void SendSms(string message, string phoneNumber)
        {
            _repository.Add(new SmsMessage {Message = message, PhoneNumber = phoneNumber });
            _unitOfWork.SaveChanges();
        }
    }
}
