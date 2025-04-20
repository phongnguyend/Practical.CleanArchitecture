using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Services.Notification.Entities;

namespace ClassifiedAds.Services.Notification.Repositories;

public interface ISmsMessageRepository : IRepository<SmsMessage, Guid>
{
    Task<int> ArchiveMessagesAsync(CancellationToken cancellationToken = default);
}
