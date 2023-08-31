using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.MessageBrokers.Fake;

public class FakeReceiver<TConsumer, T> : IMessageReceiver<TConsumer, T>
{
    public Task ReceiveAsync(Func<T, MetaData, Task> action, CancellationToken cancellationToken)
    {
        // do nothing
        return Task.CompletedTask;
    }
}
