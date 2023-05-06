using ClassifiedAds.Application.AuditLogEntries.DTOs;
using ClassifiedAds.Blazor.Modules.AuditLogs.Services;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassifiedAds.Blazor.Modules.AuditLogs.Pages;

public partial class List
{
    [Inject]
    public AuditLogService AuditLogService { get; set; }

    public List<AuditLogEntryDTO> AuditLogs { get; set; } = new List<AuditLogEntryDTO>();

    public int CurrentPage { get; set; } = 1;

    public int PageSize { get; set; } = 5;

    public long TotalItems { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var auditLogs = await AuditLogService.GetAuditLogsAsync(1, PageSize);
        AuditLogs = auditLogs.Items;
        TotalItems = auditLogs.TotalItems;
    }

    protected async Task GetAuditLogsAsync(int page)
    {
        if (page != CurrentPage)
        {
            CurrentPage = page;
            var auditLogs = await AuditLogService.GetAuditLogsAsync(page, PageSize);
            AuditLogs = auditLogs.Items;
            TotalItems = auditLogs.TotalItems;
        }
    }
}
