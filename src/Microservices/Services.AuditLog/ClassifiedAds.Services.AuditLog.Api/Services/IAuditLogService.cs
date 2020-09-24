using ClassifiedAds.Services.AuditLog.Contracts.DTOs;

namespace ClassifiedAds.Services.AuditLog.Contracts.Services
{
    public interface IAuditLogService
    {
        void AddOrUpdate(AuditLogEntryDTO auditLog);
    }
}
