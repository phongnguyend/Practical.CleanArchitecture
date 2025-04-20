using ClassifiedAds.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Domain.Repositories;

public interface ISmsMessageRepository : IRepository<SmsMessage, Guid>
{
    Task<int> ArchiveMessagesAsync(CancellationToken cancellationToken = default);
}
