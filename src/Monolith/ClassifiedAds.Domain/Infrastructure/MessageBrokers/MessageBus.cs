using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Domain.Infrastructure.MessageBrokers;

public class MessageBus : IMessageBus
{
    private readonly IServiceProvider _serviceProvider;

    public MessageBus(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task SendAsync<T>(T message, MetaData metaData = null, CancellationToken cancellationToken = default)
        where T : IMessageBusMessage
    {
        await _serviceProvider.GetRequiredService<IMessageSender<T>>().SendAsync(message, metaData, cancellationToken);
    }

    public async Task ReceiveAsync<TConsumer, T>(Func<T, MetaData, Task> action, CancellationToken cancellationToken = default)
        where T : IMessageBusMessage
    {
        await _serviceProvider.GetRequiredService<IMessageReceiver<TConsumer, T>>().ReceiveAsync(action, cancellationToken);
    }
}
