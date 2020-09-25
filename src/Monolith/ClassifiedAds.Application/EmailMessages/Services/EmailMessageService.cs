using ClassifiedAds.Application.EmailMessages.DTOs;
using ClassifiedAds.CrossCuttingConcerns.OS;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Domain.Notification;
using ClassifiedAds.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace ClassifiedAds.Application.EmailMessages.Services
{
    public class EmailMessageService
    {
        private readonly ILogger _logger;
        private readonly IEmailMessageRepository _repository;
        private readonly IMessageSender<EmailMessageCreatedEvent> _emailMessageCreatedEventSender;
        private readonly IEmailNotification _emailNotification;
        private readonly IDateTimeProvider _dateTimeProvider;

        public EmailMessageService(ILogger<EmailMessageService> logger,
            IEmailMessageRepository repository,
            IMessageSender<EmailMessageCreatedEvent> emailMessageCreatedEventSender,
            IEmailNotification emailNotification,
            IDateTimeProvider dateTimeProvider
            )
        {
            _logger = logger;
            _repository = repository;
            _emailMessageCreatedEventSender = emailMessageCreatedEventSender;
            _emailNotification = emailNotification;
            _dateTimeProvider = dateTimeProvider;
        }

        public int ResendEmailMessages()
        {
            var dateTime = _dateTimeProvider.OffsetNow.AddMinutes(-1);

            var messages = _repository.GetAll()
                .Where(x => x.SentDateTime == null && x.RetriedCount < 3)
                .Where(x => (x.RetriedCount == 0 && x.CreatedDateTime < dateTime) || (x.RetriedCount != 0 && x.UpdatedDateTime < dateTime))
                .ToList();

            if (messages.Any())
            {
                foreach (var email in messages)
                {
                    _emailMessageCreatedEventSender.Send(new EmailMessageCreatedEvent { Id = email.Id });

                    _repository.IncreaseRetry(email.Id);
                }
            }
            else
            {
                _logger.LogInformation("No email to resend.");
            }

            return messages.Count;
        }

        public void SendEmailMessage(Guid id)
        {
            var emailMessage = _repository.GetAll().FirstOrDefault(x => x.Id == id);
            if (emailMessage != null && !emailMessage.SentDateTime.HasValue)
            {
                try
                {
                    _emailNotification.Send(emailMessage);
                    _repository.UpdateSent(emailMessage.Id);
                }
                catch (Exception ex)
                {
                    _repository.UpdateFailed(emailMessage.Id, Environment.NewLine + Environment.NewLine + ex.ToString());
                }
            }
        }
    }
}
