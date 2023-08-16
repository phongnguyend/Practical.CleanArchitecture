using ClassifiedAds.CrossCuttingConcerns.DateTimes;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Infrastructure.Persistence;

namespace ClassifiedAds.Modules.Identity.Repositories;

public class Repository<T, TKey> : DbContextRepository<IdentityDbContext, T, TKey>
    where T : Entity<TKey>, IAggregateRoot
{
    public Repository(IdentityDbContext dbContext, IDateTimeProvider dateTimeProvider)
        : base(dbContext, dateTimeProvider)
    {
    }
}
