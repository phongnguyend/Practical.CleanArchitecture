using ClassifiedAds.Contracts.Identity.Services;
using ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;
using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Modules.Product.Constants;
using ClassifiedAds.Modules.Product.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.Product.EventHandlers;

public class ProductDeletedEventHandler : IDomainEventHandler<EntityDeletedEvent<Entities.Product>>
{
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<AuditLogEntry, Guid> _auditLogRepository;
    private readonly IRepository<OutboxMessage, Guid> _outboxMessageRepository;

    public ProductDeletedEventHandler(ICurrentUser currentUser,
        IRepository<AuditLogEntry, Guid> auditLogRepository,
        IRepository<OutboxMessage, Guid> outboxMessageRepository)
    {
        _currentUser = currentUser;
        _auditLogRepository = auditLogRepository;
        _outboxMessageRepository = outboxMessageRepository;
    }

    public async Task HandleAsync(EntityDeletedEvent<Entities.Product> domainEvent, CancellationToken cancellationToken = default)
    {
        var auditLog = new AuditLogEntry
        {
            UserId = _currentUser.UserId,
            CreatedDateTime = domainEvent.EventDateTime,
            Action = "DELETED_PRODUCT",
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
            EventType = EventTypeConstants.ProductDeleted,
            TriggeredById = _currentUser.UserId,
            CreatedDateTime = domainEvent.EventDateTime,
            ObjectId = domainEvent.Entity.Id.ToString(),
            Payload = domainEvent.Entity.AsJsonString(),
        }, cancellationToken);

        await _outboxMessageRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
