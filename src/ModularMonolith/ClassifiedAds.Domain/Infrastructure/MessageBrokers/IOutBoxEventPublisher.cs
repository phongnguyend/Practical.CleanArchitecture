using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Domain.Infrastructure.MessageBrokers;

public interface IOutBoxEventPublisher
{
    static abstract string[] CanHandleEventTypes();

    static abstract string CanHandleEventSource();

    Task HandleAsync(PublishingOutBoxEvent outbox, CancellationToken cancellationToken = default);
}

public class PublishingOutBoxEvent
{
    public string Id { get; set; }

    public string EventType { get; set; }

    public string EventSource { get; set; }

    public string Payload { get; set; }
}