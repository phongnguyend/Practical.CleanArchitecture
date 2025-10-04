using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Services.Notification.Entities;

namespace ClassifiedAds.Services.Notification.Persistence;

public interface IEmailMessageRepository : IRepository<EmailMessage, Guid>
{
    Task<int> ArchiveMessagesAsync(CancellationToken cancellationToken = default);
}
