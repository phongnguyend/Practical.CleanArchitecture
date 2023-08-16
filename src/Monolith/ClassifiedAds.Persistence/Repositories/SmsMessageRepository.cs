using ClassifiedAds.CrossCuttingConcerns.DateTimes;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;
using System;

namespace ClassifiedAds.Persistence.Repositories;

public class SmsMessageRepository : Repository<SmsMessage, Guid>, ISmsMessageRepository
{
    public SmsMessageRepository(AdsDbContext dbContext,
        IDateTimeProvider dateTimeProvider)
        : base(dbContext, dateTimeProvider)
    {
    }
}
