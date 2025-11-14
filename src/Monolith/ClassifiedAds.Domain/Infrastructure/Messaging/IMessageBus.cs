using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Domain.Infrastructure.Messaging;

public interface IMessageBus
{
    Task SendAsync<T>(T message, MetaData metaData = null, CancellationToken cancellationToken = default)
        where T : IMessageBusMessage;

    Task ReceiveAsync<TConsumer, T>(Func<T, MetaData, CancellationToken, Task> action, CancellationToken cancellationToken = default)
        where T : IMessageBusMessage;

    Task ReceiveAsync<TConsumer, T>(CancellationToken cancellationToken = default)
        where T : IMessageBusMessage;

    Task SendAsync(PublishingOutboxMessage outbox, CancellationToken cancellationToken = default);
}

public interface IMessageBusMessage
{
}

public interface IMessageBusEvent : IMessageBusMessage
{
    public string EventType { get; }
}

public interface IMessageBusCommand : IMessageBusMessage
{
}