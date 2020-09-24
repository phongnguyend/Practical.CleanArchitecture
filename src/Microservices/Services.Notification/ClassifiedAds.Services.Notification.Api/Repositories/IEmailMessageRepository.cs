using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Services.Notification.Entities;
using System;

namespace ClassifiedAds.Services.Notification.Repositories
{
    public interface IEmailMessageRepository : IRepository<EmailMessage, Guid>
    {
        void IncreaseRetry(Guid id);

        void UpdateSent(Guid id);

        void UpdateFailed(Guid id, string log);
    }
}
