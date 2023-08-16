using ClassifiedAds.CrossCuttingConcerns.DateTimes;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Infrastructure.Persistence;

namespace ClassifiedAds.Services.Notification.Repositories;

public class Repository<T, TKey> : DbContextRepository<NotificationDbContext, T, TKey> 
    where T : Entity<TKey>, IAggregateRoot
{
    public Repository(NotificationDbContext dbContext, IDateTimeProvider dateTimeProvider)
        : base(dbContext, dateTimeProvider)
    {
    }
}
