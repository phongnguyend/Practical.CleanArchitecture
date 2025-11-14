using ClassifiedAds.Domain.Constants;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Infrastructure.Messaging;

namespace ClassifiedAds.Application.Products.MessageBusEvents;

public class ProductDeletedEvent : IMessageBusEvent
{
    public string EventType => EventTypeConstants.ProductDeleted;

    public Product Product { get; set; }
}
