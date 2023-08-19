using ClassifiedAds.CrossCuttingConcerns.DateTimes;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Services.Product.DTOs;
using ClassifiedAds.Services.Product.Entities;
using Dapr.Client;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Product.Commands;

public class PublishEventsCommand : IRequest
{
    public int SentEventsCount { get; set; }
}

public class PublishEventsCommandHandler : IRequestHandler<PublishEventsCommand>
{
    private readonly ILogger<PublishEventsCommandHandler> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IRepository<OutboxEvent, Guid> _outboxEventRepository;
    private readonly IMessageSender<AuditLogCreatedEvent> _auditLogCreatedEventSender;
    private readonly DaprClient _daprClient;

    public PublishEventsCommandHandler(ILogger<PublishEventsCommandHandler> logger,
        IDateTimeProvider dateTimeProvider,
        IRepository<OutboxEvent, Guid> outboxEventRepository,
        IMessageSender<AuditLogCreatedEvent> auditLogCreatedEventSender,
        DaprClient daprClient)
    {
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
        _outboxEventRepository = outboxEventRepository;
        _auditLogCreatedEventSender = auditLogCreatedEventSender;
        _daprClient = daprClient;
    }

    public async Task Handle(PublishEventsCommand command, CancellationToken cancellationToken = default)
    {
        var events = _outboxEventRepository.GetQueryableSet()
            .Where(x => !x.Published)
            .OrderBy(x => x.CreatedDateTime)
            .Take(50)
            .ToList();

        foreach (var eventLog in events)
        {
            if (eventLog.EventType == "AUDIT_LOG_ENTRY_CREATED")
            {
                var logEntry = JsonSerializer.Deserialize<AuditLogEntry>(eventLog.Message);
                await _auditLogCreatedEventSender.SendAsync(new AuditLogCreatedEvent { AuditLog = logEntry },
                    new MetaData
                    {
                        MessageId = eventLog.Id.ToString(),
                    });

                if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DAPR_HTTP_PORT")))
                {
                    await _daprClient.PublishEventAsync("pubsub", "AuditLogCreatedEvent", new AuditLogCreatedEvent { AuditLog = logEntry });
                }
            }
            else
            {
                // TODO: Take Note
            }

            eventLog.Published = true;
            eventLog.UpdatedDateTime = _dateTimeProvider.OffsetNow;
            await _outboxEventRepository.UnitOfWork.SaveChangesAsync();
        }

        command.SentEventsCount = events.Count;
    }
}