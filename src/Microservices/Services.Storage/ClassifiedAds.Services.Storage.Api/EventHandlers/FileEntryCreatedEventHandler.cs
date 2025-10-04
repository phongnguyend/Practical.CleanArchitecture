using ClassifiedAds.Application;
using ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;
using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Infrastructure.Identity;
using ClassifiedAds.Services.Storage.Commands;
using ClassifiedAds.Services.Storage.Constants;
using ClassifiedAds.Services.Storage.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Storage.EventHandlers;

public class FileEntryCreatedEventHandler : IDomainEventHandler<EntityCreatedEvent<FileEntry>>
{
    private readonly Dispatcher _dispatcher;
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<OutboxMessage, Guid> _outboxMessageRepository;

    public FileEntryCreatedEventHandler(Dispatcher dispatcher,
        ICurrentUser currentUser,
        IRepository<OutboxMessage, Guid> outboxMessageRepository)
    {
        _dispatcher = dispatcher;
        _currentUser = currentUser;
        _outboxMessageRepository = outboxMessageRepository;
    }

    public async Task HandleAsync(EntityCreatedEvent<FileEntry> domainEvent, CancellationToken cancellationToken = default)
    {
        await _dispatcher.DispatchAsync(new AddAuditLogEntryCommand
        {
            AuditLogEntry = new AuditLogEntry
            {
                UserId = _currentUser.IsAuthenticated ? _currentUser.UserId : Guid.Empty,
                CreatedDateTime = domainEvent.EventDateTime,
                Action = "CREATED_FILEENTRY",
                ObjectId = domainEvent.Entity.Id.ToString(),
                Log = domainEvent.Entity.AsJsonString(),
            },
        });

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
