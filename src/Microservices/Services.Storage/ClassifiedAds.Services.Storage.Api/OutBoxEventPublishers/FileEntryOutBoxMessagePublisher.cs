using ClassifiedAds.Domain.Infrastructure.Messaging;
using ClassifiedAds.Services.Storage.Commands;
using ClassifiedAds.Services.Storage.Constants;
using ClassifiedAds.Services.Storage.DTOs;
using ClassifiedAds.Services.Storage.Entities;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Storage.OutboxMessagePublishers;

public class FileEntryOutboxMessagePublisher : IOutboxMessagePublisher
{
    private readonly IMessageBus _messageBus;

    public static string[] CanHandleEventTypes()
    {
        return [EventTypeConstants.FileEntryCreated, EventTypeConstants.FileEntryDeleted];
    }

    public static string CanHandleEventSource()
    {
        return typeof(PublishEventsCommand).Assembly.GetName().Name;
    }

    public FileEntryOutboxMessagePublisher(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task HandleAsync(PublishingOutboxMessage outbox, CancellationToken cancellationToken = default)
    {
        if (outbox.EventType == EventTypeConstants.FileEntryCreated)
        {
            await _messageBus.SendAsync(new FileUploadedEvent
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