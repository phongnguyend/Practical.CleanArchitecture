using ClassifiedAds.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Domain.Repositories;

public interface IEmailMessageRepository : IRepository<EmailMessage, Guid>
{
    Task<int> ArchiveMessagesAsync(CancellationToken cancellationToken = default);
}
