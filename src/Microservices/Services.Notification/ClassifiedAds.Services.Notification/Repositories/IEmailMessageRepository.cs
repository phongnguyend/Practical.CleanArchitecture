using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Services.Notification.Entities;

namespace ClassifiedAds.Services.Notification.Repositories;

public interface IEmailMessageRepository : IRepository<EmailMessage, Guid>
{
    Task<int> ArchiveMessagesAsync(CancellationToken cancellationToken = default);
}
