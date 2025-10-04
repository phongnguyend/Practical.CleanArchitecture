using ClassifiedAds.CrossCuttingConcerns.DateTimes;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Persistence.SqlServer;

namespace ClassifiedAds.Modules.Configuration.Persistence;

public class Repository<T, TKey> : DbContextRepository<ConfigurationDbContext, T, TKey>
    where T : Entity<TKey>, IAggregateRoot
{
    public Repository(ConfigurationDbContext dbContext, IDateTimeProvider dateTimeProvider)
        : base(dbContext, dateTimeProvider)
    {
    }
}
