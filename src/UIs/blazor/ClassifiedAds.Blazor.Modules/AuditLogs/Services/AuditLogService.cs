using ClassifiedAds.Application.AuditLogEntries.DTOs;
using ClassifiedAds.Application.Common.DTOs;
using ClassifiedAds.Blazor.Modules.Core.Services;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClassifiedAds.Blazor.Modules.AuditLogs.Services;

public class AuditLogService : HttpService
{
    public AuditLogService(HttpClient httpClient, ITokenManager tokenManager) 
        : base(httpClient, tokenManager)
    {
    }

    public async Task<List<AuditLogEntryDTO>> GetAuditLogsAsync()
    {
        var logs = await GetAsync<List<AuditLogEntryDTO>>("api/auditLogEntries");
        return logs;
    }

    public async Task<Paged<AuditLogEntryDTO>> GetAuditLogsAsync(int page, int pageSize)
    {
        var logs = await GetAsync<Paged<AuditLogEntryDTO>>($"api/auditLogEntries/paged?page={page}&pageSize={pageSize}");
        return logs;
    }
}
