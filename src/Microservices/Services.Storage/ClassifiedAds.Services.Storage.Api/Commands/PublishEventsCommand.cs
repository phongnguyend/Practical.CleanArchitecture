using ClassifiedAds.Application;
using ClassifiedAds.CrossCuttingConcerns.DateTimes;
using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Services.Storage.DTOs;
using ClassifiedAds.Services.Storage.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Storage.Commands;

public class PublishEventsCommand : ICommand
{
    public int SentEventsCount { get; set; }
}

public class PublishEventsCommandHandler : ICommandHandler<PublishEventsCommand>
{
    private readonly ILogger<PublishEventsCommandHandler> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IRepository<OutboxEvent, Guid> _outboxEventRepository;
    private readonly IMessageSender<FileUploadedEvent> _fileUploadedEventSender;
    private readonly IMessageSender<FileDeletedEvent> _fileDeletedEventSender;
    private readonly IMessageSender<AuditLogCreatedEvent> _auditLogCreatedEventSender;

    public PublishEventsCommandHandler(ILogger<PublishEventsCommandHandler> logger,
        IDateTimeProvider dateTimeProvider,
        IRepository<OutboxEvent, Guid> outboxEventRepository,
        IMessageSender<FileUploadedEvent> fileUploadedEventSender,
        IMessageSender<FileDeletedEvent> fileDeletedEventSender,
        IMessageSender<AuditLogCreatedEvent> auditLogCreatedEventSender)
    {
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
        _outboxEventRepository = outboxEventRepository;
        _fileUploadedEventSender = fileUploadedEventSender;
        _fileDeletedEventSender = fileDeletedEventSender;
        _auditLogCreatedEventSender = auditLogCreatedEventSender;
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
            if (eventLog.EventType == "FILEENTRY_CREATED")
            {
                await _fileUploadedEventSender.SendAsync(new FileUploadedEvent { FileEntry = JsonSerializer.Deserialize<FileEntry>(eventLog.Message) });
            }
            else if (eventLog.EventType == "FILEENTRY_DELETED")
            {
                await _fileDeletedEventSender.SendAsync(new FileDeletedEvent { FileEntry = JsonSerializer.Deserialize<FileEntry>(eventLog.Message) });
            }
            else if (eventLog.EventType == "AUDIT_LOG_ENTRY_CREATED")
            {
                var logEntry = JsonSerializer.Deserialize<AuditLogEntry>(eventLog.Message);
                await _auditLogCreatedEventSender.SendAsync(new AuditLogCreatedEvent { AuditLog = logEntry },
                    new MetaData
                    {
                        MessageId = eventLog.Id.ToString(),
                    });
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