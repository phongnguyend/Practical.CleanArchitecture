using ClassifiedAds.Application.AuditLogEntries.DTOs;
using ClassifiedAds.Blazor.Modules.AuditLogs.Services;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassifiedAds.Blazor.Modules.AuditLogs.Pages
{
    public partial class List
    {
        [Inject]
        public AuditLogService AuditLogService { get; set; }

        public List<AuditLogEntryDTO> AuditLogs { get; set; } = new List<AuditLogEntryDTO>();

        protected override async Task OnInitializedAsync()
        {
            AuditLogs = await AuditLogService.GetAuditLogs();
        }
    }
}
