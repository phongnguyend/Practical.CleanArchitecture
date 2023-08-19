using ClassifiedAds.Contracts.AuditLog.DTOs;

namespace ClassifiedAds.Contracts.AuditLog.Services;

public interface IAuditLogService
{
    Task AddAsync(AuditLogEntryDTO auditLog, string requestId);

    Task<List<AuditLogEntryDTO>> GetAuditLogEntriesAsync(AuditLogEntryQueryOptions query);
}
