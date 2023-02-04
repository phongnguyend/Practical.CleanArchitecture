using ClassifiedAds.CrossCuttingConcerns.OS;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;
using System;

namespace ClassifiedAds.Persistence.Repositories
{
    public class EmailMessageRepository : Repository<EmailMessage, Guid>, IEmailMessageRepository
    {
        public EmailMessageRepository(AdsDbContext dbContext,
            IDateTimeProvider dateTimeProvider)
            : base(dbContext, dateTimeProvider)
        {
        }
    }
}
