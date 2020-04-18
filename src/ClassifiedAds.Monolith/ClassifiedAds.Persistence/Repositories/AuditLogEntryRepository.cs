using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ClassifiedAds.Persistence.Repositories
{
    public class AuditLogEntryRepository : Repository<AuditLogEntry, Guid>, IAuditLogEntryRepository
    {
        public AuditLogEntryRepository(AdsDbContext dbContext)
            : base(dbContext)
        {
        }

        public IQueryable<AuditLogEntry> Get(AuditLogEntryQueryOptions queryOptions)
        {
            var query = GetAll();

            if (queryOptions.UserId != Guid.Empty)
            {
                query = query.Where(x => x.UserId == queryOptions.UserId);
            }

            if (!string.IsNullOrEmpty(queryOptions.ObjectId))
            {
                query = query.Where(x => x.ObjectId == queryOptions.ObjectId);
            }

            if (queryOptions.AsNoTracking)
            {
                query = query.AsNoTracking();
            }

            return query;
        }
    }
}
