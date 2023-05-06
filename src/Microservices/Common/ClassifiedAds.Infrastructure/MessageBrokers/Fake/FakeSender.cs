using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.MessageBrokers.Fake;

public class FakeSender<T> : IMessageSender<T>
{
    public Task SendAsync(T message, MetaData metaData = null, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
