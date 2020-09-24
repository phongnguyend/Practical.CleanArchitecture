using ClassifiedAds.Services.AuditLog.DTOs;

namespace ClassifiedAds.Services.AuditLog.Services
{
    public interface IAuditLogService
    {
        void AddOrUpdate(AuditLogEntryDTO auditLog);
    }
}
