using ClassifiedAds.CrossCuttingConcerns.DateTimes;
using ClassifiedAds.Modules.Notification.Entities;
using System;

namespace ClassifiedAds.Modules.Notification.Repositories;

public class SmsMessageRepository : Repository<SmsMessage, Guid>, ISmsMessageRepository
{
    public SmsMessageRepository(NotificationDbContext dbContext,
        IDateTimeProvider dateTimeProvider)
        : base(dbContext, dateTimeProvider)
    {
    }
}
