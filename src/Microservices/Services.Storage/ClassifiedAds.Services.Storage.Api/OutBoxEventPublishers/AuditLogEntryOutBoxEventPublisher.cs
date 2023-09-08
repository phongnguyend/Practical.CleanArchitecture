using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Services.Storage.Commands;
using ClassifiedAds.Services.Storage.Constants;
using ClassifiedAds.Services.Storage.DTOs;
using ClassifiedAds.Services.Storage.Entities;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Storage.OutBoxEventPublishers;

public class AuditLogEntryOutBoxEventPublisher : IOutBoxEventPublisher
{
    private readonly IMessageBus _messageBus;

    public static string[] CanHandleEventTypes()
    {
        return new string[] { EventTypeConstants.AuditLogEntryCreated };
    }

    public static string CanHandleEventSource()
    {
        return typeof(PublishEventsCommand).Assembly.GetName().Name;
    }

    public AuditLogEntryOutBoxEventPublisher(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task HandleAsync(PublishingOutBoxEvent outbox, CancellationToken cancellationToken = default)
    {
        if (outbox.EventType == EventTypeConstants.AuditLogEntryCreated)
        {
            var logEntry = JsonSerializer.Deserialize<AuditLogEntry>(outbox.Payload);
            await _messageBus.SendAsync(new AuditLogCreatedEvent { AuditLog = logEntry },
                new MetaData
                {
                    MessageId = outbox.Id,
                });
        }
    }
}