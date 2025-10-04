using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Services.Notification.Entities;

namespace ClassifiedAds.Services.Notification.Persistence;

public interface ISmsMessageRepository : IRepository<SmsMessage, Guid>
{
    Task<int> ArchiveMessagesAsync(CancellationToken cancellationToken = default);
}
