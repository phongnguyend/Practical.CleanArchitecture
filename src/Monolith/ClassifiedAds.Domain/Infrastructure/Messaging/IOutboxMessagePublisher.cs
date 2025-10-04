using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Domain.Infrastructure.Messaging;

public interface IOutboxMessagePublisher
{
    static abstract string[] CanHandleEventTypes();

    static abstract string CanHandleEventSource();

    Task HandleAsync(PublishingOutboxMessage outbox, CancellationToken cancellationToken = default);
}

public class PublishingOutboxMessage
{
    public string Id { get; set; }

    public string EventType { get; set; }

    public string EventSource { get; set; }

    public string Payload { get; set; }

    public string ActivityId { get; set; }
}