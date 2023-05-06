using ClassifiedAds.Modules.AuditLog.Contracts.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.AuditLog.Contracts.Services;

public interface IAuditLogService
{
    Task AddAsync(AuditLogEntryDTO auditLog);

    Task<List<AuditLogEntryDTO>> GetAuditLogEntriesAsync(AuditLogEntryQueryOptions query);
}
