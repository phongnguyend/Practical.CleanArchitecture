using ClassifiedAds.Modules.AuditLog.Contracts.DTOs;
using System.Collections.Generic;

namespace ClassifiedAds.Modules.AuditLog.Contracts.Services
{
    public interface IAuditLogService
    {
        void AddOrUpdate(AuditLogEntryDTO auditLog);

        List<AuditLogEntryDTO> GetAuditLogEntries(AuditLogEntryQueryOptions query);
    }
}
