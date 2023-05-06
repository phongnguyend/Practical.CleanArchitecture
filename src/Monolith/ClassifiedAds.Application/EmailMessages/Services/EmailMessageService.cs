using ClassifiedAds.CrossCuttingConcerns.CircuitBreakers;
using ClassifiedAds.CrossCuttingConcerns.Locks;
using ClassifiedAds.CrossCuttingConcerns.OS;
using ClassifiedAds.Domain.Notification;
using ClassifiedAds.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ClassifiedAds.Application.EmailMessages.Services;

public class EmailMessageService
{
    private readonly ILogger _logger;
    private readonly IEmailMessageRepository _repository;
    private readonly IEmailNotification _emailNotification;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICircuitBreakerManager _circuitBreakerManager;
    private readonly IDistributedLock _distributedLock;

    public EmailMessageService(ILogger<EmailMessageService> logger,
        IEmailMessageRepository repository,
        IEmailNotification emailNotification,
        IDateTimeProvider dateTimeProvider,
        ICircuitBreakerManager circuitBreakerManager,
        IDistributedLock distributedLock)
    {
        _logger = logger;
        _repository = repository;
        _emailNotification = emailNotification;
        _dateTimeProvider = dateTimeProvider;
        _circuitBreakerManager = circuitBreakerManager;
        _distributedLock = distributedLock;
    }

    public async Task<int> SendEmailMessagesAsync()
    {
        var circuit = _circuitBreakerManager.GetCircuitBreaker("EmailService", TimeSpan.FromMinutes(1));
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
            foreach (var email in messages)
            {
                string log = Environment.NewLine + Environment.NewLine
                        + $"[{_dateTimeProvider.OffsetNow.ToString(CultureInfo.InvariantCulture)}] ";
                try
                {
                    await _emailNotification.SendAsync(email);
                    email.SentDateTime = _dateTimeProvider.OffsetNow;
                    email.Log += log + "Succeed.";

                    _circuitBreakerManager.LogSuccess(circuit);
                }
                catch (Exception ex)
                {
                    email.Log += log + ex.ToString();
                    email.NextAttemptDateTime = _dateTimeProvider.OffsetNow + deplayedTimes[email.AttemptCount];

                    _circuitBreakerManager.LogFailure(circuit, 5, TimeSpan.FromMinutes(5));
                }

                email.AttemptCount += 1;
                email.Log = email.Log.Trim();
                email.UpdatedDateTime = _dateTimeProvider.OffsetNow;

                if (email.MaxAttemptCount == 0)
                {
                    email.MaxAttemptCount = defaultAttemptCount;
                }

                await _repository.UnitOfWork.SaveChangesAsync();

                circuit.EnsureOkStatus();
            }
        }
        else
        {
            _logger.LogInformation("No email to send.");
        }

        return messages.Count;
    }
}
