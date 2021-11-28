using ClassifiedAds.CrossCuttingConcerns.OS;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Infrastructure.Persistence;

namespace ClassifiedAds.Services.Notification.Repositories
{
    public class Repository<T, TKey> : DbContextRepository<NotificationDbContext, T, TKey> 
        where T : AggregateRoot<TKey>
    {
        public Repository(NotificationDbContext dbContext, IDateTimeProvider dateTimeProvider)
            : base(dbContext, dateTimeProvider)
        {
        }
    }
}
