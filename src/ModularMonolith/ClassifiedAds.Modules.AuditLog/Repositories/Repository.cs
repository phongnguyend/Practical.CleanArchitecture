using ClassifiedAds.CrossCuttingConcerns.OS;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Infrastructure.Persistence;

namespace ClassifiedAds.Modules.AuditLog.Repositories
{
    public class Repository<T, TKey> : DbContextRepository<AuditLogDbContext, T, TKey>
        where T : AggregateRoot<TKey>
    {
        public Repository(AuditLogDbContext dbContext, IDateTimeProvider dateTimeProvider)
            : base(dbContext, dateTimeProvider)
        {
        }
    }
}
