using ClassifiedAds.CrossCuttingConcerns.OS;
using ClassifiedAds.Domain.Notification;
using ClassifiedAds.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ClassifiedAds.Application.EmailMessages.Services
{
    public class EmailMessageService
    {
        private readonly ILogger _logger;
        private readonly IEmailMessageRepository _repository;
        private readonly IEmailNotification _emailNotification;
        private readonly IDateTimeProvider _dateTimeProvider;

        public EmailMessageService(ILogger<EmailMessageService> logger,
            IEmailMessageRepository repository,
            IEmailNotification emailNotification,
            IDateTimeProvider dateTimeProvider
            )
        {
            _logger = logger;
            _repository = repository;
            _emailNotification = emailNotification;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<int> SendEmailMessagesAsync()
        {
            var dateTime = _dateTimeProvider.OffsetNow.AddMinutes(-1);

            var messages = _repository.GetAll()
                .Where(x => x.SentDateTime == null && x.RetriedCount < 3)
                .Where(x => (x.RetriedCount == 0) || (x.RetriedCount != 0 && x.UpdatedDateTime < dateTime))
                .ToList();

            if (messages.Any())
            {
                foreach (var email in messages)
                {
                    try
                    {
                        await _emailNotification.SendAsync(email);
                        _repository.UpdateSent(email.Id);
                    }
                    catch (Exception ex)
                    {
                        _repository.UpdateFailed(email.Id, Environment.NewLine + Environment.NewLine + ex.ToString());
                        _repository.IncreaseRetry(email.Id);
                    }
                }
            }
            else
            {
                _logger.LogInformation("No email to send.");
            }

            return messages.Count;
        }
    }
}
