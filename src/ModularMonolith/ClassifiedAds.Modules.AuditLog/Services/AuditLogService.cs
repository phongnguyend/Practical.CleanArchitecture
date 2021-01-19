using ClassifiedAds.Application;
using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Modules.AuditLog.Contracts.DTOs;
using ClassifiedAds.Modules.AuditLog.Contracts.Services;
using ClassifiedAds.Modules.AuditLog.Entities;
using ClassifiedAds.Modules.AuditLog.Queries;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.AuditLog.Services
{
    public class AuditLogService : CrudService<AuditLogEntry>, IAuditLogService
    {
        private readonly Dispatcher _dispatcher;

        public AuditLogService(IRepository<AuditLogEntry, Guid> repository, IDomainEvents domainEvents, Dispatcher dispatcher)
            : base(repository, domainEvents)
        {
            _dispatcher = dispatcher;
        }

        public async Task AddOrUpdateAsync(AuditLogEntryDTO dto)
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
}
