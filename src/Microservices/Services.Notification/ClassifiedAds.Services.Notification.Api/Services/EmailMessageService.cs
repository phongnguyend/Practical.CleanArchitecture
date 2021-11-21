using ClassifiedAds.Application;
using ClassifiedAds.CrossCuttingConcerns.OS;
using ClassifiedAds.Domain.Events;
using ClassifiedAds.Infrastructure.Notification.Email;
using ClassifiedAds.Services.Notification.DTOs;
using ClassifiedAds.Services.Notification.Entities;
using ClassifiedAds.Services.Notification.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
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
            var deplayedTimes = new[]
            {
                TimeSpan.FromMinutes(1),
                TimeSpan.FromMinutes(2),
                TimeSpan.FromMinutes(3),
                TimeSpan.FromMinutes(5),
                TimeSpan.FromMinutes(8),
                TimeSpan.FromMinutes(13),
                TimeSpan.FromMinutes(21),
                TimeSpan.FromMinutes(34),
                TimeSpan.FromMinutes(55),
                TimeSpan.FromMinutes(89),
            };

            var dateTime = _dateTimeProvider.OffsetNow;
            var defaultAttemptCount = 5;

            var messages = _repository.GetAll()
                .Where(x => x.SentDateTime == null)
                .Where(x => x.ExpiredDateTime == null || x.ExpiredDateTime > dateTime)
                .Where(x => (x.MaxAttemptCount == 0 && x.AttemptCount < defaultAttemptCount) || x.AttemptCount < x.MaxAttemptCount)
                .Where(x => x.NextAttemptDateTime == null || x.NextAttemptDateTime <= dateTime)
                .ToList();

            if (messages.Any())
            {
                foreach (var email in messages)
                {
                    string log = Environment.NewLine + Environment.NewLine
                            + $"[{_dateTimeProvider.OffsetNow.ToString(CultureInfo.InvariantCulture)}] ";
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

                        email.SentDateTime = _dateTimeProvider.OffsetNow;
                        email.Log += log + "Succeed.";
                    }
                    catch (Exception ex)
                    {
                        email.Log += log + ex.ToString();
                        email.NextAttemptDateTime = _dateTimeProvider.OffsetNow + deplayedTimes[email.AttemptCount];
                    }

                    email.AttemptCount += 1;
                    email.Log = email.Log.Trim();
                    email.UpdatedDateTime = _dateTimeProvider.OffsetNow;

                    if (email.MaxAttemptCount == 0)
                    {
                        email.MaxAttemptCount = defaultAttemptCount;
                    }

                    await _repository.UnitOfWork.SaveChangesAsync();
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
