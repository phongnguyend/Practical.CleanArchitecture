using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Domain.Infrastructure.Messaging;

public interface IMessageSender<T>
{
    Task SendAsync(T message, MetaData metaData = null, CancellationToken cancellationToken = default);
}
