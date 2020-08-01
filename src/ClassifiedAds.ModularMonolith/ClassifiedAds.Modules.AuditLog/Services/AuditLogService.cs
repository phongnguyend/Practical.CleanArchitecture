using ClassifiedAds.Application;
using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Modules.AuditLog.Contracts.DTOs;
using ClassifiedAds.Modules.AuditLog.Contracts.Services;
using ClassifiedAds.Modules.AuditLog.Entities;
using System;

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
    }
}
