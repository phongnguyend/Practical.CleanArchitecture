using ClassifiedAds.Application;
using ClassifiedAds.CrossCuttingConcerns.DateTimes;
using ClassifiedAds.Infrastructure.Notification.Email;
using ClassifiedAds.Modules.Notification.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.Notification.Commands;

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

    public SendEmailMessagesCommandHandler(ILogger<SendEmailMessagesCommandHandler> logger,
        IEmailMessageRepository repository,
        IEmailNotification emailNotification,
        IDateTimeProvider dateTimeProvider)
    {
        _logger = logger;
        _repository = repository;
        _emailNotification = emailNotification;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task HandleAsync(SendEmailMessagesCommand command, CancellationToken cancellationToken = default)
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
                    await _emailNotification.SendAsync(new DTOs.EmailMessageDTO
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

        command.SentMessagesCount = messages.Count;
    }
}
