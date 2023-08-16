using ClassifiedAds.CrossCuttingConcerns.CircuitBreakers;
using ClassifiedAds.CrossCuttingConcerns.DateTimes;
using ClassifiedAds.Domain.Notification;
using ClassifiedAds.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Application.EmailMessages.Commands;

public class SendEmailMessagesCommand : ICommand
{
    public int SentMessagesCount { get; set; }
}

public class SendEmailMessagesCommandHandler : ICommandHandler<SendEmailMessagesCommand>
{
    private readonly ILogger _logger;
    private readonly IEmailMessageRepository _repository;
    private readonly IEmailNotification _emailNotification;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICircuitBreakerManager _circuitBreakerManager;

    public SendEmailMessagesCommandHandler(ILogger<SendEmailMessagesCommandHandler> logger,
        IEmailMessageRepository repository,
        IEmailNotification emailNotification,
        IDateTimeProvider dateTimeProvider,
        ICircuitBreakerManager circuitBreakerManager)
    {
        _logger = logger;
        _repository = repository;
        _emailNotification = emailNotification;
        _dateTimeProvider = dateTimeProvider;
        _circuitBreakerManager = circuitBreakerManager;
    }

    public async Task HandleAsync(SendEmailMessagesCommand command, CancellationToken cancellationToken = default)
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

        var messages = _repository.GetQueryableSet()
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
                    await _emailNotification.SendAsync(email, cancellationToken);
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

                await _repository.UnitOfWork.SaveChangesAsync(cancellationToken);

                circuit.EnsureOkStatus();
            }
        }
        else
        {
            _logger.LogInformation("No email to send.");
        }

        command.SentMessagesCount = messages.Count;
    }
}
