using ClassifiedAds.Application.EventLogs.Commands;
using ClassifiedAds.Application.FileEntries.DTOs;
using ClassifiedAds.Domain.Constants;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.BackgroundServer.OutBoxEventPublishers;

public class FileEntryOutBoxEventPublisher : IOutBoxEventPublisher
{
    private readonly IMessageBus _messageBus;

    public static string[] CanHandleEventTypes()
    {
        return new string[] { EventTypeConstants.FileEntryCreated, EventTypeConstants.FileEntryDeleted };
    }

    public static string CanHandleEventSource()
    {
        return typeof(PublishEventsCommand).Assembly.GetName().Name;
    }

    public FileEntryOutBoxEventPublisher(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task HandleAsync(PublishingOutBoxEvent outbox, CancellationToken cancellationToken = default)
    {
        if (outbox.EventType == EventTypeConstants.FileEntryCreated)
        {
            await _messageBus.SendAsync(new FileUploadedEvent { FileEntry = JsonSerializer.Deserialize<FileEntry>(outbox.Payload) }, cancellationToken: cancellationToken);
        }
        else if (outbox.EventType == EventTypeConstants.FileEntryDeleted)
        {
            await _messageBus.SendAsync(new FileDeletedEvent { FileEntry = JsonSerializer.Deserialize<FileEntry>(outbox.Payload) }, cancellationToken: cancellationToken);
        }
    }
}
