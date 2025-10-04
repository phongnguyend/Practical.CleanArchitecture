using ClassifiedAds.CrossCuttingConcerns.DateTimes;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Persistence.SqlServer;

namespace ClassifiedAds.Services.Product.Persistence;

public class Repository<T, TKey> : DbContextRepository<ProductDbContext, T, TKey>
    where T : Entity<TKey>, IAggregateRoot
{
    public Repository(ProductDbContext dbContext, IDateTimeProvider dateTimeProvider)
        : base(dbContext, dateTimeProvider)
    {
    }
}
