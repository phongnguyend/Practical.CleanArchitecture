﻿using ClassifiedAds.CrossCuttingConcerns.CircuitBreakers;
using ClassifiedAds.CrossCuttingConcerns.Locks;
using ClassifiedAds.CrossCuttingConcerns.OS;
using ClassifiedAds.Domain.Notification;
using ClassifiedAds.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ClassifiedAds.Application.SmsMessages.Services
{
    public class SmsMessageService
    {
        private readonly ILogger _logger;
        private readonly ISmsMessageRepository _repository;
        private readonly ISmsNotification _smsNotification;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICircuitBreakerManager _circuitBreakerManager;
        private readonly IDistributedLock _distributedLock;

        public SmsMessageService(ILogger<SmsMessageService> logger,
            ISmsMessageRepository repository,
            ISmsNotification smsNotification,
            IDateTimeProvider dateTimeProvider,
            ICircuitBreakerManager circuitBreakerManager,
            IDistributedLock distributedLock)
        {
            _logger = logger;
            _repository = repository;
            _smsNotification = smsNotification;
            _dateTimeProvider = dateTimeProvider;
            _circuitBreakerManager = circuitBreakerManager;
            _distributedLock = distributedLock;
        }

        public async Task<int> SendSmsMessagesAsync()
        {
            var circuit = _circuitBreakerManager.GetCircuitBreaker("SmsService", TimeSpan.FromMinutes(1));
            circuit.EnsureOkStatus();

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
                foreach (var sms in messages)
                {
                    string log = Environment.NewLine + Environment.NewLine
                            + $"[{_dateTimeProvider.OffsetNow.ToString(CultureInfo.InvariantCulture)}] ";
                    try
                    {
                        await _smsNotification.SendAsync(sms);
                        sms.SentDateTime = _dateTimeProvider.OffsetNow;
                        sms.Log += log + "Succeed.";

                        _circuitBreakerManager.LogSuccess(circuit);
                    }
                    catch (Exception ex)
                    {
                        sms.Log += log + ex.ToString();
                        sms.NextAttemptDateTime = _dateTimeProvider.OffsetNow + deplayedTimes[sms.AttemptCount];

                        _circuitBreakerManager.LogFailure(circuit, 5, TimeSpan.FromMinutes(5));
                    }

                    sms.AttemptCount += 1;
                    sms.Log = sms.Log.Trim();
                    sms.UpdatedDateTime = _dateTimeProvider.OffsetNow;

                    if (sms.MaxAttemptCount == 0)
                    {
                        sms.MaxAttemptCount = defaultAttemptCount;
                    }

                    await _repository.UnitOfWork.SaveChangesAsync();

                    circuit.EnsureOkStatus();
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
