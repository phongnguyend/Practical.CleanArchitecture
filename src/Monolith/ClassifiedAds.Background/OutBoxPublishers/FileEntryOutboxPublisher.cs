using ClassifiedAds.Application.FileEntries.MessageBusEvents;
using ClassifiedAds.Application.OutboxMessages.Commands;
using ClassifiedAds.Domain.Constants;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Infrastructure.Messaging;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Background.OutBoxPublishers;

public class FileEntryOutboxPublisher : IOutboxMessagePublisher
{
    private readonly IMessageBus _messageBus;

    public static string[] CanHandleEventTypes()
    {
        return [EventTypeConstants.FileEntryCreated, EventTypeConstants.FileEntryUpdated, EventTypeConstants.FileEntryDeleted];
    }

    public static string CanHandleEventSource()
    {
        return typeof(PublishOutboxMessagesCommand).Assembly.GetName().Name;
    }

    public FileEntryOutboxPublisher(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task HandleAsync(PublishingOutboxMessage outbox, CancellationToken cancellationToken = default)
    {
        if (outbox.EventType == EventTypeConstants.FileEntryCreated)
        {
            await _messageBus.SendAsync(new FileCreatedEvent
            {
                FileEntry = JsonSerializer.Deserialize<FileEntry>(outbox.Payload),
            },
            metaData: new MetaData { ActivityId = outbox.ActivityId, MessageId = outbox.Id },
            cancellationToken: cancellationToken);
        }
        else if (outbox.EventType == EventTypeConstants.FileEntryUpdated)
        {
            await _messageBus.SendAsync(new FileUpdatedEvent
            {
                FileEntry = JsonSerializer.Deserialize<FileEntry>(outbox.Payload)
            },
            metaData: new MetaData { ActivityId = outbox.ActivityId, MessageId = outbox.Id },
            cancellationToken: cancellationToken);
        }
        else if (outbox.EventType == EventTypeConstants.FileEntryDeleted)
        {
            await _messageBus.SendAsync(new FileDeletedEvent
            {
                FileEntry = JsonSerializer.Deserialize<FileEntry>(outbox.Payload)
            },
            metaData: new MetaData { ActivityId = outbox.ActivityId, MessageId = outbox.Id },
            cancellationToken: cancellationToken);
        }
    }
}
