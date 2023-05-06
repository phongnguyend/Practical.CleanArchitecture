using ClassifiedAds.Services.AuditLog.Entities;

namespace ClassifiedAds.Services.AuditLog.DTOs;

public class AuditLogCreatedEvent
{
    public AuditLogEntry AuditLog { get; set; }
}
