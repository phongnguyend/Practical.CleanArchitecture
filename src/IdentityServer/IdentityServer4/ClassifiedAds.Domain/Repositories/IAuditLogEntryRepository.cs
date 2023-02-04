using ClassifiedAds.Domain.Entities;
using System;
using System.Linq;

namespace ClassifiedAds.Domain.Repositories
{
    public class AuditLogEntryQueryOptions
    {
        public Guid UserId { get; set; }

        public string ObjectId { get; set; }

        public bool AsNoTracking { get; set; }
    }

    public interface IAuditLogEntryRepository : IRepository<AuditLogEntry, Guid>
    {
        IQueryable<AuditLogEntry> Get(AuditLogEntryQueryOptions queryOptions);
    }
}
