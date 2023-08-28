using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Domain.Infrastructure.MessageBrokers;

public interface IMessageReceiver<TConsumer, T>
{
    Task ReceiveAsync(Func<T, MetaData, Task> action, CancellationToken cancellationToken);
}
