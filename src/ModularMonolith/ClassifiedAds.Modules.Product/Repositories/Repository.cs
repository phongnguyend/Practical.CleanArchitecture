using ClassifiedAds.CrossCuttingConcerns.DateTimes;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Infrastructure.Persistence;

namespace ClassifiedAds.Modules.Product.Repositories;

public class Repository<T, TKey> : DbContextRepository<ProductDbContext, T, TKey>
    where T : Entity<TKey>, IAggregateRoot
{
    public Repository(ProductDbContext dbContext, IDateTimeProvider dateTimeProvider)
        : base(dbContext, dateTimeProvider)
    {
    }
}
