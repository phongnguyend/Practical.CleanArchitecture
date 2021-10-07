using ClassifiedAds.CrossCuttingConcerns.OS;
using ClassifiedAds.Infrastructure.Notification.Sms;
using ClassifiedAds.Modules.Notification.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.Notification.Services
{
    public class SmsMessageService
    {
        private readonly ILogger _logger;
        private readonly ISmsMessageRepository _repository;
        private readonly ISmsNotification _smsNotification;
        private readonly IDateTimeProvider _dateTimeProvider;

        public SmsMessageService(ILogger<SmsMessageService> logger,
            ISmsMessageRepository repository,
            ISmsNotification smsNotification,
            IDateTimeProvider dateTimeProvider)
        {
            _logger = logger;
            _repository = repository;
            _smsNotification = smsNotification;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<int> SendSmsMessagesAsync()
        {
            var dateTime = _dateTimeProvider.OffsetNow.AddMinutes(-1);

            var messages = _repository.GetAll()
                .Where(x => x.SentDateTime == null && x.RetriedCount < 3)
                .Where(x => (x.RetriedCount == 0) || (x.RetriedCount != 0 && x.UpdatedDateTime < dateTime))
                .ToList();

            if (messages.Any())
            {
                foreach (var sms in messages)
                {
                    try
                    {
                        await _smsNotification.SendAsync(new DTOs.SmsMessageDTO
                        {
                            Message = sms.Message,
                            PhoneNumber = sms.PhoneNumber,
                        });

                        _repository.UpdateSent(sms.Id);
                    }
                    catch (Exception ex)
                    {
                        _repository.UpdateFailed(sms.Id, Environment.NewLine + Environment.NewLine + ex.ToString());
                        _repository.IncreaseRetry(sms.Id);
                    }
                }
            }
            else
            {
                _logger.LogInformation("No SMS to send.");
            }

            return messages.Count;
        }
    }
}
