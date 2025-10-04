using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Modules.Notification.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.Notification.Persistence;

public interface ISmsMessageRepository : IRepository<SmsMessage, Guid>
{
    Task<int> ArchiveMessagesAsync(CancellationToken cancellationToken = default);
}
