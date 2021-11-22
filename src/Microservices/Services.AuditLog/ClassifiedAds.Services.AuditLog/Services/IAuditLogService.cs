using ClassifiedAds.Services.AuditLog.DTOs;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.AuditLog.Services
{
    public interface IAuditLogService
    {
        Task AddOrUpdateAsync(AuditLogEntryDTO auditLog);
    }
}
