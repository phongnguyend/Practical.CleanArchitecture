using ClassifiedAds.CrossCuttingConcerns.OS;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Infrastructure.Persistence;

namespace ClassifiedAds.Services.Configuration.Repositories
{
    public class Repository<T, TKey> : DbContextRepository<ConfigurationDbContext, T, TKey>
    where T : AggregateRoot<TKey>
    {
        public Repository(ConfigurationDbContext dbContext, IDateTimeProvider dateTimeProvider)
            : base(dbContext, dateTimeProvider)
        {
        }
    }
}
