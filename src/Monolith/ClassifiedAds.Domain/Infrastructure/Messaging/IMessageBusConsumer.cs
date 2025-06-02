using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Domain.Infrastructure.Messaging;

public interface IMessageBusConsumer<TConsumer, T>
{
    Task HandleAsync(T data, MetaData metaData, CancellationToken cancellationToken = default);
}
