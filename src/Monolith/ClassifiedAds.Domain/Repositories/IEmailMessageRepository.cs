using ClassifiedAds.Domain.Entities;
using System;

namespace ClassifiedAds.Domain.Repositories
{
    public interface IEmailMessageRepository : IRepository<EmailMessage, Guid>
    {
        void IncreaseRetry(Guid id);

        void UpdateSent(Guid id);

        void UpdateFailed(Guid id, string log);
    }
}
