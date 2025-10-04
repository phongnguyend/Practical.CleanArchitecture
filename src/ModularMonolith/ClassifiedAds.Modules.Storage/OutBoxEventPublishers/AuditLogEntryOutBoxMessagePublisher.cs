using ClassifiedAds.Contracts.AuditLog.Services;
using ClassifiedAds.Domain.Infrastructure.Messaging;
using ClassifiedAds.Modules.Storage.Commands;
using ClassifiedAds.Modules.Storage.Constants;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.Storage.OutboxMessagePublishers;

public class AuditLogEntryOutboxMessagePublisher : IOutboxMessagePublisher
{
    private readonly IMessageBus _messageBus;
    private readonly IAuditLogService _externalAuditLogService;

    public static string[] CanHandleEventTypes()
    {
        return new string[] { EventTypeConstants.AuditLogEntryCreated };
    }

    public static string CanHandleEventSource()
    {
        return typeof(PublishEventsCommand).Assembly.GetName().Name;
    }

    public AuditLogEntryOutboxMessagePublisher(IMessageBus messageBus,
        IAuditLogService externalAuditLogService)
    {
        _messageBus = messageBus;
        _externalAuditLogService = externalAuditLogService;
    }

    public async Task HandleAsync(PublishingOutboxMessage outbox, CancellationToken cancellationToken = default)
    {
        if (outbox.EventType == EventTypeConstants.AuditLogEntryCreated)
        {
            var logEntry = JsonSerializer.Deserialize<Contracts.AuditLog.DTOs.AuditLogEntryDTO>(outbox.Payload);
            await _externalAuditLogService.AddAsync(logEntry, outbox.Id);
        }
    }
}