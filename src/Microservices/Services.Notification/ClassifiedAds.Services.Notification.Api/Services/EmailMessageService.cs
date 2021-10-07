using ClassifiedAds.Application;
using ClassifiedAds.CrossCuttingConcerns.OS;
using ClassifiedAds.Domain.Events;
using ClassifiedAds.Infrastructure.Notification.Email;
using ClassifiedAds.Services.Notification.DTOs;
using ClassifiedAds.Services.Notification.Entities;
using ClassifiedAds.Services.Notification.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Notification.Services
{
    public class EmailMessageService : CrudService<EmailMessage>, IEmailMessageService
    {
        private readonly ILogger _logger;
        private readonly IEmailMessageRepository _repository;
        private readonly IEmailNotification _emailNotification;
        private readonly IDateTimeProvider _dateTimeProvider;

        public EmailMessageService(ILogger<EmailMessageService> logger,
            IEmailMessageRepository repository,
            IDomainEvents domainEvents,
            IEmailNotification emailNotification,
            IDateTimeProvider dateTimeProvider)
            : base(repository, domainEvents)
        {
            _logger = logger;
            _repository = repository;
            _emailNotification = emailNotification;
            _dateTimeProvider = dateTimeProvider;
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
                        await _emailNotification.SendAsync(new EmailMessageDTO
                        {
                            From = email.From,
                            Tos = email.Tos,
                            CCs = email.CCs,
                            BCCs = email.BCCs,
                            Subject = email.Subject,
                            Body = email.Body,
                        });

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
