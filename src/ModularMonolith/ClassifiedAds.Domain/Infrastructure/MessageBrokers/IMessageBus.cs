using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Domain.Infrastructure.MessageBrokers;

public interface IMessageBus
{
    Task SendAsync<T>(T message, MetaData metaData = null, CancellationToken cancellationToken = default)
        where T : IMessageBusEvent;

    Task ReceiveAsync<TConsumer, T>(Func<T, MetaData, Task> action, CancellationToken cancellationToken = default)
        where T : IMessageBusEvent;
}
