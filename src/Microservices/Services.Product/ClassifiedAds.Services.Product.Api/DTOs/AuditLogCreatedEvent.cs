using ClassifiedAds.Services.Product.Entities;

namespace ClassifiedAds.Services.Product.DTOs;

public class AuditLogCreatedEvent
{
    public AuditLogEntry AuditLog { get; set; }
}
