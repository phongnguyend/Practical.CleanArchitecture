using ClassifiedAds.CrossCuttingConcerns.DateTimes;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Infrastructure.Persistence;

namespace ClassifiedAds.Services.AuditLog.Repositories;

public class Repository<T, TKey> : DbContextRepository<AuditLogDbContext, T, TKey>
    where T : Entity<TKey>, IAggregateRoot
{
    public Repository(AuditLogDbContext dbContext, IDateTimeProvider dateTimeProvider)
        : base(dbContext, dateTimeProvider)
    {
    }
}
