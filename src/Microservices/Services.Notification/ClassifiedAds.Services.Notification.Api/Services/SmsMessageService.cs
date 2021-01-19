﻿using ClassifiedAds.CrossCuttingConcerns.OS;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Infrastructure.Notification.Sms;
using ClassifiedAds.Services.Notification.DTOs;
using ClassifiedAds.Services.Notification.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Notification.Services
{
    public class SmsMessageService
    {
        private readonly ILogger _logger;
        private readonly ISmsMessageRepository _repository;
        private readonly IMessageSender<SmsMessageCreatedEvent> _smsMessageCreatedEventSender;
        private readonly ISmsNotification _smsNotification;
        private readonly IDateTimeProvider _dateTimeProvider;

        public SmsMessageService(ILogger<SmsMessageService> logger,
            ISmsMessageRepository repository,
            IMessageSender<SmsMessageCreatedEvent> smsMessageCreatedEventSender,
            ISmsNotification smsNotification,
            IDateTimeProvider dateTimeProvider)
        {
            _logger = logger;
            _repository = repository;
            _smsMessageCreatedEventSender = smsMessageCreatedEventSender;
            _smsNotification = smsNotification;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<int> ResendSmsMessageAsync()
        {
            var dateTime = _dateTimeProvider.OffsetNow.AddMinutes(-1);

            var messages = _repository.GetAll()
                .Where(x => x.SentDateTime == null && x.RetriedCount < 3)
                .Where(x => (x.RetriedCount == 0 && x.CreatedDateTime < dateTime) || (x.RetriedCount != 0 && x.UpdatedDateTime < dateTime))
                .ToList();

            if (messages.Any())
            {
                foreach (var sms in messages)
                {
                    await _smsMessageCreatedEventSender.SendAsync(new SmsMessageCreatedEvent { Id = sms.Id });

                    sms.RetriedCount++;

                    await _repository.AddOrUpdateAsync(sms);
                    await _repository.UnitOfWork.SaveChangesAsync();
                }
            }
            else
            {
                _logger.LogInformation("No SMS to resend.");
            }

            return messages.Count;
        }

        public async Task SendSmsMessageAsync(Guid id)
        {
            var smsMessage = _repository.GetAll().FirstOrDefault(x => x.Id == id);
            if (smsMessage != null && !smsMessage.SentDateTime.HasValue)
            {
                try
                {
                    await _smsNotification.SendAsync(new SmsMessageDTO
                    {
                        Message = smsMessage.Message,
                        PhoneNumber = smsMessage.PhoneNumber,
                    });

                    _repository.UpdateSent(smsMessage.Id);
                }
                catch (Exception ex)
                {
                    _repository.UpdateFailed(smsMessage.Id, Environment.NewLine + Environment.NewLine + ex.ToString());
                }
            }
        }
    }
}
