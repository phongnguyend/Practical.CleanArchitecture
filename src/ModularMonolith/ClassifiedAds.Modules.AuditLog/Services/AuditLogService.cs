using ClassifiedAds.Application;
using ClassifiedAds.Contracts.AuditLog.DTOs;
using ClassifiedAds.Contracts.AuditLog.Services;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Modules.AuditLog.Entities;
using ClassifiedAds.Modules.AuditLog.Queries;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.AuditLog.Services;

public class AuditLogService : CrudService<AuditLogEntry>, IAuditLogService
{
    public AuditLogService(IRepository<AuditLogEntry, Guid> repository, Dispatcher dispatcher)
        : base(repository, dispatcher)
    {
    }

    public async Task AddAsync(AuditLogEntryDTO dto)
    {
        await AddOrUpdateAsync(new AuditLogEntry
        {
            UserId = dto.UserId,
            CreatedDateTime = dto.CreatedDateTime,
            Action = dto.Action,
            ObjectId = dto.ObjectId,
            Log = dto.Log,
        });
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
