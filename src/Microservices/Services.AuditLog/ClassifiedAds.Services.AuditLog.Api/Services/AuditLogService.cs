using ClassifiedAds.Application;
using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Services.AuditLog.DTOs;
using ClassifiedAds.Services.AuditLog.Entities;
using System;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.AuditLog.Services
{
    public class AuditLogService : CrudService<AuditLogEntry>, IAuditLogService
    {
        private readonly Dispatcher _dispatcher;

        public AuditLogService(IRepository<AuditLogEntry, Guid> repository, IDomainEvents domainEvents, Dispatcher dispatcher)
            : base(repository, domainEvents)
        {
            _dispatcher = dispatcher;
        }

        public Task AddOrUpdateAsync(AuditLogEntryDTO dto)
        {
            return AddOrUpdateAsync(new AuditLogEntry
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
