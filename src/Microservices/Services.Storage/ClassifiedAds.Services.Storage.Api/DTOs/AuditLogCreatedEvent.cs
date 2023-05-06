using ClassifiedAds.Services.Storage.Entities;

namespace ClassifiedAds.Services.Storage.DTOs;

public class AuditLogCreatedEvent
{
    public AuditLogEntry AuditLog { get; set; }
}
