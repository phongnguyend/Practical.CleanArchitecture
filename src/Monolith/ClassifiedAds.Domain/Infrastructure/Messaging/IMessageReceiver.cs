using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Domain.Infrastructure.Messaging;

public interface IMessageReceiver<TConsumer, T>
{
    Task ReceiveAsync(Func<T, MetaData, CancellationToken, Task> action, CancellationToken cancellationToken);
}

public class ConsumerHandledException : Exception
{
    public ConsumerHandledExceptionNextAction NextAction { get; set; }
}

public enum ConsumerHandledExceptionNextAction
{
    Retry,
    ReQueue,
    DeadLetter,
}
