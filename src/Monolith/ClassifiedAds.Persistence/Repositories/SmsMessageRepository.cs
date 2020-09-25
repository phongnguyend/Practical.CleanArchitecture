using ClassifiedAds.CrossCuttingConcerns.OS;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;

namespace ClassifiedAds.Persistence.Repositories
{
    public class SmsMessageRepository : Repository<SmsMessage, Guid>, ISmsMessageRepository
    {
        public SmsMessageRepository(AdsDbContext dbContext,
            IDateTimeProvider dateTimeProvider)
            : base(dbContext, dateTimeProvider)
        {
        }

        public void IncreaseRetry(Guid id)
        {
            _dbContext.Database
                .ExecuteSqlRaw("Update SmsMessages set RetriedCount = RetriedCount + 1, UpdatedDateTime = SYSDATETIMEOFFSET() where Id = @id",
                new SqlParameter("id", id));
        }

        public void UpdateSent(Guid id)
        {
            _dbContext.Database
                .ExecuteSqlRaw("Update SmsMessages set SentDateTime = SYSDATETIMEOFFSET() where Id = @id",
                new SqlParameter("id", id));
        }

        public void UpdateFailed(Guid id, string log)
        {
            _dbContext.Database
                .ExecuteSqlRaw("Update SmsMessages set UpdatedDateTime = SYSDATETIMEOFFSET(), Log = ISNULL(Log, '') + @log where Id = @id",
                   new SqlParameter("id", id),
                   new SqlParameter("log", log));
        }
    }
}
