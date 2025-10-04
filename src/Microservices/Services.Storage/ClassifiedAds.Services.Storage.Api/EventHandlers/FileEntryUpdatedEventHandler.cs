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

public class FileEntryUpdatedEventHandler : IDomainEventHandler<EntityUpdatedEvent<FileEntry>>
{
    private readonly Dispatcher _dispatcher;
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<OutboxMessage, Guid> _outboxMessageRepository;

    public FileEntryUpdatedEventHandler(Dispatcher dispatcher,
        ICurrentUser currentUser,
        IRepository<OutboxMessage, Guid> outboxMessageRepository)
    {
        _dispatcher = dispatcher;
        _currentUser = currentUser;
        _outboxMessageRepository = outboxMessageRepository;
    }

    public async Task HandleAsync(EntityUpdatedEvent<FileEntry> domainEvent, CancellationToken cancellationToken = default)
    {
        await _dispatcher.DispatchAsync(new AddAuditLogEntryCommand
        {
            AuditLogEntry = new AuditLogEntry
            {
                UserId = _currentUser.IsAuthenticated ? _currentUser.UserId : Guid.Empty,
                CreatedDateTime = domainEvent.EventDateTime,
                Action = "UPDATED_FILEENTRY",
                ObjectId = domainEvent.Entity.Id.ToString(),
                Log = domainEvent.Entity.AsJsonString(),
            },
        });

        await _outboxMessageRepository.AddOrUpdateAsync(new OutboxMessage
        {
            EventType = EventTypeConstants.FileEntryUpdated,
            TriggeredById = _currentUser.UserId,
            CreatedDateTime = domainEvent.EventDateTime,
            ObjectId = domainEvent.Entity.Id.ToString(),
            Payload = domainEvent.Entity.AsJsonString(),
        }, cancellationToken);

        await _outboxMessageRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
