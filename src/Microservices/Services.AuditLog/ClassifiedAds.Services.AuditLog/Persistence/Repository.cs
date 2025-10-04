using ClassifiedAds.CrossCuttingConcerns.DateTimes;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Persistence.SqlServer;

namespace ClassifiedAds.Services.AuditLog.Persistence;

public class Repository<T, TKey> : DbContextRepository<AuditLogDbContext, T, TKey>
    where T : Entity<TKey>, IAggregateRoot
{
    public Repository(AuditLogDbContext dbContext, IDateTimeProvider dateTimeProvider)
        : base(dbContext, dateTimeProvider)
    {
    }
}
