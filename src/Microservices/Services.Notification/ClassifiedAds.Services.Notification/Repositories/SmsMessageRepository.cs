using ClassifiedAds.CrossCuttingConcerns.OS;
using ClassifiedAds.Services.Notification.Entities;
using System;

namespace ClassifiedAds.Services.Notification.Repositories
{
    public class SmsMessageRepository : Repository<SmsMessage, Guid>, ISmsMessageRepository
    {
        public SmsMessageRepository(NotificationDbContext dbContext,
            IDateTimeProvider dateTimeProvider)
            : base(dbContext, dateTimeProvider)
        {
        }
    }
}
