using ClassifiedAds.Domain.Entities;
using System;

namespace ClassifiedAds.Domain.Repositories
{
    public interface ISmsMessageRepository : IRepository<SmsMessage, Guid>
    {
        void IncreaseRetry(Guid id);

        void UpdateSent(Guid id);

        void UpdateFailed(Guid id, string log);
    }
}
