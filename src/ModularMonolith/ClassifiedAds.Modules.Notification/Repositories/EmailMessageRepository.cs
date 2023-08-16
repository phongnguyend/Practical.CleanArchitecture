using ClassifiedAds.CrossCuttingConcerns.DateTimes;
using ClassifiedAds.Modules.Notification.Entities;
using System;

namespace ClassifiedAds.Modules.Notification.Repositories;

public class EmailMessageRepository : Repository<EmailMessage, Guid>, IEmailMessageRepository
{
    public EmailMessageRepository(NotificationDbContext dbContext,
        IDateTimeProvider dateTimeProvider)
        : base(dbContext, dateTimeProvider)
    {
    }
}
