using ClassifiedAds.Application;
using ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Infrastructure.Identity;
using ClassifiedAds.Services.Product.Constants;
using ClassifiedAds.Services.Product.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;
namespace ClassifiedAds.Services.Product.Commands;

public class AddAuditLogEntryCommand : ICommand
{
    public AuditLogEntry AuditLogEntry { get; set; }
}

public class AddAuditLogEntryCommandHandler : ICommandHandler<AddAuditLogEntryCommand>
{
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<AuditLogEntry, Guid> _auditLogRepository;
    private readonly IRepository<OutboxMessage, Guid> _outboxMessageRepository;

    public AddAuditLogEntryCommandHandler(
        ICurrentUser currentUser,
        IRepository<AuditLogEntry, Guid> auditLogRepository,
        IRepository<OutboxMessage, Guid> outboxMessageRepository)
    {
        _currentUser = currentUser;
        _auditLogRepository = auditLogRepository;
        _outboxMessageRepository = outboxMessageRepository;
    }

    public async Task HandleAsync(AddAuditLogEntryCommand command, CancellationToken cancellationToken)
    {
        var auditLog = new AuditLogEntry
        {
            UserId = command.AuditLogEntry.UserId,
            CreatedDateTime = command.AuditLogEntry.CreatedDateTime,
            Action = command.AuditLogEntry.Action,
            ObjectId = command.AuditLogEntry.ObjectId,
            Log = command.AuditLogEntry.Log,
        };

        await _auditLogRepository.AddOrUpdateAsync(auditLog);
        await _auditLogRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        await _outboxMessageRepository.AddOrUpdateAsync(new OutboxMessage
        {
            EventType = EventTypeConstants.AuditLogEntryCreated,
            TriggeredById = _currentUser.UserId,
            CreatedDateTime = auditLog.CreatedDateTime,
            ObjectId = auditLog.Id.ToString(),
            Payload = auditLog.AsJsonString(),
        }, cancellationToken);

        await _outboxMessageRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
