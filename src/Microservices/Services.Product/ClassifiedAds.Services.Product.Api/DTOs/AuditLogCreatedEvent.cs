using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Services.Product.Entities;

namespace ClassifiedAds.Services.Product.DTOs;

public class AuditLogCreatedEvent : IMessageBusEvent
{
    public AuditLogEntry AuditLog { get; set; }
}
