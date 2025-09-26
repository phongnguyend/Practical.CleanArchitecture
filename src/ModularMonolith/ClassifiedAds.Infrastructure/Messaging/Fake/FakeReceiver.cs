using ClassifiedAds.Domain.Infrastructure.Messaging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Messaging.Fake;

public class FakeReceiver<TConsumer, T> : IMessageReceiver<TConsumer, T>
{
    public Task ReceiveAsync(Func<T, MetaData, CancellationToken, Task> action, CancellationToken cancellationToken)
    {
        // do nothing
        return Task.CompletedTask;
    }
}
