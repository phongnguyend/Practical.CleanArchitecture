using ClassifiedAds.Contracts.Identity.Services;
using ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;
using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Modules.Storage.Constants;
using ClassifiedAds.Modules.Storage.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.Storage.EventHandlers;

public class FileEntryCreatedEventHandler : IDomainEventHandler<EntityCreatedEvent<FileEntry>>
{
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<AuditLogEntry, Guid> _auditLogRepository;
    private readonly IRepository<OutboxMessage, Guid> _outboxMessageRepository;

    public FileEntryCreatedEventHandler(ICurrentUser currentUser,
        IRepository<AuditLogEntry, Guid> auditLogRepository,
        IRepository<OutboxMessage, Guid> outboxMessageRepository)
    {
        _currentUser = currentUser;
        _auditLogRepository = auditLogRepository;
        _outboxMessageRepository = outboxMessageRepository;
    }

    public async Task HandleAsync(EntityCreatedEvent<FileEntry> domainEvent, CancellationToken cancellationToken = default)
    {
        var auditLog = new AuditLogEntry
        {
            UserId = _currentUser.IsAuthenticated ? _currentUser.UserId : Guid.Empty,
            CreatedDateTime = domainEvent.EventDateTime,
            Action = "CREATED_FILEENTRY",
            ObjectId = domainEvent.Entity.Id.ToString(),
            Log = domainEvent.Entity.AsJsonString(),
        };

        await _auditLogRepository.AddOrUpdateAsync(auditLog, cancellationToken);
        await _auditLogRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        await _outboxMessageRepository.AddOrUpdateAsync(new OutboxMessage
        {
            EventType = EventTypeConstants.AuditLogEntryCreated,
            TriggeredById = _currentUser.UserId,
            CreatedDateTime = auditLog.CreatedDateTime,
            ObjectId = auditLog.Id.ToString(),
            Payload = auditLog.AsJsonString(),
        }, cancellationToken);

        await _outboxMessageRepository.AddOrUpdateAsync(new OutboxMessage
        {
            EventType = EventTypeConstants.FileEntryCreated,
            TriggeredById = _currentUser.UserId,
            CreatedDateTime = domainEvent.EventDateTime,
            ObjectId = domainEvent.Entity.Id.ToString(),
            Payload = domainEvent.Entity.AsJsonString(),
        }, cancellationToken);

        await _outboxMessageRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
