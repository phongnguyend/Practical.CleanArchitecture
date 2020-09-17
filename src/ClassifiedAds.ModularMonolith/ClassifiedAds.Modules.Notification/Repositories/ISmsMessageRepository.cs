using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Modules.Notification.Entities;
using System;

namespace ClassifiedAds.Modules.Notification.Repositories
{
    public interface ISmsMessageRepository : IRepository<SmsMessage, Guid>
    {
        void IncreaseRetry(Guid id);

        void UpdateSent(Guid id);

        void UpdateFailed(Guid id, string log);
    }
}
