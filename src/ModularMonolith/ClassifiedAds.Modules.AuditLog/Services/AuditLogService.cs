using ClassifiedAds.Application;
using ClassifiedAds.Contracts.AuditLog.DTOs;
using ClassifiedAds.Contracts.AuditLog.Services;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Modules.AuditLog.Entities;
using ClassifiedAds.Modules.AuditLog.Queries;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.AuditLog.Services;

public class AuditLogService : CrudService<AuditLogEntry>, IAuditLogService
{
    private readonly IRepository<IdempotentRequest, Guid> _idempotentRequestRepository;

    public AuditLogService(IRepository<AuditLogEntry, Guid> repository,
        Dispatcher dispatcher,
        IRepository<IdempotentRequest, Guid> idempotentRequestRepository)
        : base(repository, dispatcher)
    {
        _idempotentRequestRepository = idempotentRequestRepository;
    }

    public async Task AddAsync(AuditLogEntryDTO dto, string requestId)
    {
        var requestType = "ADD_AUDIT_LOG_ENTRY";

        var requestProcessed = await _idempotentRequestRepository.GetQueryableSet().AnyAsync(x => x.RequestType == requestType && x.RequestId == requestId);

        if (requestProcessed)
        {
            return;
        }

        var uow = _idempotentRequestRepository.UnitOfWork;

        await uow.BeginTransactionAsync();

        await AddOrUpdateAsync(new AuditLogEntry
        {
            UserId = dto.UserId,
            CreatedDateTime = dto.CreatedDateTime,
            Action = dto.Action,
            ObjectId = dto.ObjectId,
            Log = dto.Log,
        });

        await _idempotentRequestRepository.AddAsync(new IdempotentRequest
        {
            RequestType = requestType,
            RequestId = requestId,
        });

        await uow.SaveChangesAsync();

        await uow.CommitTransactionAsync();
    }

    public async Task<List<AuditLogEntryDTO>> GetAuditLogEntriesAsync(AuditLogEntryQueryOptions query)
    {
        var logs = await _dispatcher.DispatchAsync(new GetAuditEntriesQuery
        {
            UserId = query.UserId,
            ObjectId = query.ObjectId,
            AsNoTracking = query.AsNoTracking,
        });

        return logs;
    }
}
