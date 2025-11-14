using ClassifiedAds.Application.OutboxMessages.Commands;
using ClassifiedAds.Application.Products.MessageBusEvents;
using ClassifiedAds.Domain.Constants;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Infrastructure.Messaging;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Background.OutBoxPublishers;

public class ProductOutboxPublisher : IOutboxMessagePublisher
{
    private readonly IMessageBus _messageBus;

    public static string[] CanHandleEventTypes()
    {
        return [EventTypeConstants.ProductCreated, EventTypeConstants.ProductUpdated, EventTypeConstants.ProductDeleted];
    }

    public static string CanHandleEventSource()
    {
        return typeof(PublishOutboxMessagesCommand).Assembly.GetName().Name;
    }

    public ProductOutboxPublisher(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task HandleAsync(PublishingOutboxMessage outbox, CancellationToken cancellationToken = default)
    {
        if (outbox.EventType == EventTypeConstants.ProductCreated)
        {
            await _messageBus.SendAsync(new ProductCreatedEvent
            {
                Product = JsonSerializer.Deserialize<Product>(outbox.Payload),
            },
            metaData: new MetaData { ActivityId = outbox.ActivityId, MessageId = outbox.Id },
            cancellationToken: cancellationToken);
        }
        else if (outbox.EventType == EventTypeConstants.ProductUpdated)
        {
            await _messageBus.SendAsync(new ProductUpdatedEvent
            {
                Product = JsonSerializer.Deserialize<Product>(outbox.Payload)
            },
            metaData: new MetaData { ActivityId = outbox.ActivityId, MessageId = outbox.Id },
            cancellationToken: cancellationToken);
        }
        else if (outbox.EventType == EventTypeConstants.ProductDeleted)
        {
            await _messageBus.SendAsync(new ProductDeletedEvent
            {
                Product = JsonSerializer.Deserialize<Product>(outbox.Payload)
            },
            metaData: new MetaData { ActivityId = outbox.ActivityId, MessageId = outbox.Id },
            cancellationToken: cancellationToken);
        }
    }
}
