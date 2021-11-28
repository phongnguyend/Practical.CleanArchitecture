using ClassifiedAds.CrossCuttingConcerns.OS;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Infrastructure.Persistence;

namespace ClassifiedAds.Modules.Identity.Repositories
{
    public class Repository<T, TKey> : DbContextRepository<IdentityDbContext, T, TKey>
        where T : AggregateRoot<TKey>
    {
        public Repository(IdentityDbContext dbContext, IDateTimeProvider dateTimeProvider)
            : base(dbContext, dateTimeProvider)
        {
        }
    }
}
