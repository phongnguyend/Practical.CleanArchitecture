using ClassifiedAds.Domain.Constants;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Infrastructure.Messaging;

namespace ClassifiedAds.Application.Products.MessageBusEvents;

public class ProductUpdatedEvent : IMessageBusEvent
{
    public string EventType => EventTypeConstants.ProductUpdated;

    public Product Product { get; set; }
}
