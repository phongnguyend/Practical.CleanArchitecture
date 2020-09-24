using ClassifiedAds.CrossCuttingConcerns.OS;
using ClassifiedAds.Services.Notification.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;

namespace ClassifiedAds.Services.Notification.Repositories
{
    public class EmailMessageRepository : Repository<EmailMessage, Guid>, IEmailMessageRepository
    {
        public EmailMessageRepository(NotificationDbContext dbContext,
            IDateTimeProvider dateTimeProvider)
            : base(dbContext, dateTimeProvider)
        {
        }

        public void IncreaseRetry(Guid id)
        {
            _dbContext.Database
                .ExecuteSqlRaw("Update EmailMessages set RetriedCount = RetriedCount + 1, UpdatedDateTime = SYSDATETIMEOFFSET() where Id = @id",
                new SqlParameter("id", id));
        }

        public void UpdateSent(Guid id)
        {
            _dbContext.Database
                .ExecuteSqlRaw("Update EmailMessages set SentDateTime = SYSDATETIMEOFFSET() where Id = @id",
                    new SqlParameter("id", id));
        }

        public void UpdateFailed(Guid id, string log)
        {
            _dbContext.Database
                .ExecuteSqlRaw("Update EmailMessages set UpdatedDateTime = SYSDATETIMEOFFSET(), Log = ISNULL(Log, '') + @log where Id = @id",
                    new SqlParameter("id", id),
                    new SqlParameter("log", log));
        }
    }
}
