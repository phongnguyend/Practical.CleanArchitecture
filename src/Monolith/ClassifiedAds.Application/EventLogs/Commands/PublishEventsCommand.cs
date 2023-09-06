using ClassifiedAds.Application.FileEntries.DTOs;
using ClassifiedAds.CrossCuttingConcerns.DateTimes;
using ClassifiedAds.Domain.Constants;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Application.EventLogs.Commands;

public class PublishEventsCommand : ICommand
{
    public int SentEventsCount { get; set; }
}

public class PublishEventsCommandHandler : ICommandHandler<PublishEventsCommand>
{
    private readonly ILogger<PublishEventsCommandHandler> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IRepository<OutboxEvent, Guid> _outboxEventRepository;
    private readonly IMessageBus _messageBus;

    public PublishEventsCommandHandler(ILogger<PublishEventsCommandHandler> logger,
        IDateTimeProvider dateTimeProvider,
        IRepository<OutboxEvent, Guid> outboxEventRepository,
        IMessageBus messageBus)
    {
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
        _outboxEventRepository = outboxEventRepository;
        _messageBus = messageBus;
    }

    public async Task HandleAsync(PublishEventsCommand command, CancellationToken cancellationToken = default)
    {
        var events = _outboxEventRepository.GetQueryableSet()
            .Where(x => !x.Published)
            .OrderBy(x => x.CreatedDateTime)
            .Take(50)
            .ToList();

        foreach (var eventLog in events)
        {
            if (eventLog.EventType == EventTypeConstants.FileEntryCreated)
            {
                await _messageBus.SendAsync(new FileUploadedEvent { FileEntry = JsonSerializer.Deserialize<FileEntry>(eventLog.Message) }, cancellationToken: cancellationToken);
            }
            else if (eventLog.EventType == EventTypeConstants.FileEntryDeleted)
            {
                await _messageBus.SendAsync(new FileDeletedEvent { FileEntry = JsonSerializer.Deserialize<FileEntry>(eventLog.Message) }, cancellationToken: cancellationToken);
            }
            else
            {
                // TODO: Take Note
            }

            eventLog.Published = true;
            eventLog.UpdatedDateTime = _dateTimeProvider.OffsetNow;
            await _outboxEventRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        command.SentEventsCount = events.Count;
    }
}