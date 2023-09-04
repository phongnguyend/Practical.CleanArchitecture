using ClassifiedAds.Domain.Infrastructure.MessageBrokers;
using ClassifiedAds.Services.Storage.Entities;

namespace ClassifiedAds.Services.Storage.DTOs;

public class AuditLogCreatedEvent : IMessageBusEvent
{
    public AuditLogEntry AuditLog { get; set; }
}
