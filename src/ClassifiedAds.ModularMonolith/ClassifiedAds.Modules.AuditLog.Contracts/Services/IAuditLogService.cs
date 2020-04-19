using ClassifiedAds.Modules.AuditLog.Contracts.DTOs;

namespace ClassifiedAds.Modules.AuditLog.Contracts.Services
{
    public interface IAuditLogService
    {
        void AddOrUpdate(AuditLogEntryDTO auditLog);
    }
}
