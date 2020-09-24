using ClassifiedAds.Application;
using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Modules.AuditLog.Contracts.DTOs;
using ClassifiedAds.Modules.AuditLog.Contracts.Services;
using ClassifiedAds.Modules.AuditLog.Entities;
using ClassifiedAds.Modules.AuditLog.Queries;
using System;
using System.Collections.Generic;

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

        public void AddOrUpdate(AuditLogEntryDTO dto)
        {
            AddOrUpdate(new AuditLogEntry
            {
                UserId = dto.UserId,
                CreatedDateTime = dto.CreatedDateTime,
                Action = dto.Action,
                ObjectId = dto.ObjectId,
                Log = dto.Log,
            });
        }

        public List<AuditLogEntryDTO> GetAuditLogEntries(AuditLogEntryQueryOptions query)
        {
            var logs = _dispatcher.Dispatch(new GetAuditEntriesQuery
            {
                UserId = query.UserId,
                ObjectId = query.ObjectId,
                AsNoTracking = query.AsNoTracking,
            });

            return logs;
        }
    }
}
