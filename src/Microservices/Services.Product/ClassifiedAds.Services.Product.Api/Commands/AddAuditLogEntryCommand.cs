using ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Infrastructure.Identity;
using ClassifiedAds.Services.Product.Constants;
using ClassifiedAds.Services.Product.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
namespace ClassifiedAds.Services.Product.Commands;

public class AddAuditLogEntryCommand : IRequest
{
    public AuditLogEntry AuditLogEntry { get; set; }
}

public class AddAuditLogEntryCommandHandler : IRequestHandler<AddAuditLogEntryCommand>
{
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<AuditLogEntry, Guid> _auditLogRepository;
    private readonly IRepository<OutboxEvent, Guid> _outboxEventRepository;

    public AddAuditLogEntryCommandHandler(
        ICurrentUser currentUser,
        IRepository<AuditLogEntry, Guid> auditLogRepository,
        IRepository<OutboxEvent, Guid> outboxEventRepository)
    {
        _currentUser = currentUser;
        _auditLogRepository = auditLogRepository;
        _outboxEventRepository = outboxEventRepository;
    }

    public async Task Handle(AddAuditLogEntryCommand command, CancellationToken cancellationToken)
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

        await _outboxEventRepository.AddOrUpdateAsync(new OutboxEvent
        {
            EventType = EventTypeConstants.AuditLogEntryCreated,
            TriggeredById = _currentUser.UserId,
            CreatedDateTime = auditLog.CreatedDateTime,
            ObjectId = auditLog.Id.ToString(),
            Message = auditLog.AsJsonString(),
            Published = false,
        }, cancellationToken);

        await _outboxEventRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
