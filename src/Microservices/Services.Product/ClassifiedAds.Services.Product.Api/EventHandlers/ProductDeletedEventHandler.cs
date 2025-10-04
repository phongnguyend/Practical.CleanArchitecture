using ClassifiedAds.Application;
using ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;
using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Infrastructure.Identity;
using ClassifiedAds.Services.Product.Commands;
using ClassifiedAds.Services.Product.Constants;
using ClassifiedAds.Services.Product.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Product.EventHandlers;

public class ProductDeletedEventHandler : IDomainEventHandler<EntityDeletedEvent<Entities.Product>>
{
    private readonly Dispatcher _dispatcher;
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<OutboxMessage, Guid> _outboxMessageRepository;

    public ProductDeletedEventHandler(Dispatcher dispatcher,
        ICurrentUser currentUser,
        IRepository<OutboxMessage, Guid> outboxMessageRepository)
    {
        _dispatcher = dispatcher;
        _currentUser = currentUser;
        _outboxMessageRepository = outboxMessageRepository;
    }

    public async Task HandleAsync(EntityDeletedEvent<Entities.Product> domainEvent, CancellationToken cancellationToken = default)
    {
        await _dispatcher.DispatchAsync(new AddAuditLogEntryCommand
        {
            AuditLogEntry = new AuditLogEntry
            {
                UserId = _currentUser.UserId,
                CreatedDateTime = domainEvent.EventDateTime,
                Action = "DELETED_PRODUCT",
                ObjectId = domainEvent.Entity.Id.ToString(),
                Log = domainEvent.Entity.AsJsonString(),
            },
        });

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
