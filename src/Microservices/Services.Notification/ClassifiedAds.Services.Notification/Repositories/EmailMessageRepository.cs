using ClassifiedAds.CrossCuttingConcerns.DateTimes;
using ClassifiedAds.Services.Notification.Entities;
using System;

namespace ClassifiedAds.Services.Notification.Repositories;

public class EmailMessageRepository : Repository<EmailMessage, Guid>, IEmailMessageRepository
{
    public EmailMessageRepository(NotificationDbContext dbContext,
        IDateTimeProvider dateTimeProvider)
        : base(dbContext, dateTimeProvider)
    {
    }
}
