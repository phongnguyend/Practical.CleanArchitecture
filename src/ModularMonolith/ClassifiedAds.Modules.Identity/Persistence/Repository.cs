using ClassifiedAds.CrossCuttingConcerns.DateTimes;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Persistence.SqlServer;

namespace ClassifiedAds.Modules.Identity.Persistence;

public class Repository<T, TKey> : DbContextRepository<IdentityDbContext, T, TKey>
    where T : Entity<TKey>, IAggregateRoot
{
    public Repository(IdentityDbContext dbContext, IDateTimeProvider dateTimeProvider)
        : base(dbContext, dateTimeProvider)
    {
    }
}
