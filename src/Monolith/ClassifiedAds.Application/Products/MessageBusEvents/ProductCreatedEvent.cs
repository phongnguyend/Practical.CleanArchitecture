using ClassifiedAds.Domain.Constants;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Infrastructure.Messaging;

namespace ClassifiedAds.Application.Products.MessageBusEvents;

public class ProductCreatedEvent : IMessageBusEvent
{
    public string EventType => EventTypeConstants.ProductCreated;

    public Product Product { get; set; }
}
