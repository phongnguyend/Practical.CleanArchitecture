using ClassifiedAds.Contracts.AuditLog.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassifiedAds.Contracts.AuditLog.Services;

public interface IAuditLogService
{
    Task AddAsync(AuditLogEntryDTO auditLog);

    Task<List<AuditLogEntryDTO>> GetAuditLogEntriesAsync(AuditLogEntryQueryOptions query);
}
