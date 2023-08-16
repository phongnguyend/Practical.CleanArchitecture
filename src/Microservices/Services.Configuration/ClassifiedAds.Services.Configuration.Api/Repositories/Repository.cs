using ClassifiedAds.CrossCuttingConcerns.DateTimes;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Infrastructure.Persistence;

namespace ClassifiedAds.Services.Configuration.Repositories;

public class Repository<T, TKey> : DbContextRepository<ConfigurationDbContext, T, TKey>
where T : Entity<TKey>, IAggregateRoot
{
    public Repository(ConfigurationDbContext dbContext, IDateTimeProvider dateTimeProvider)
        : base(dbContext, dateTimeProvider)
    {
    }
}
