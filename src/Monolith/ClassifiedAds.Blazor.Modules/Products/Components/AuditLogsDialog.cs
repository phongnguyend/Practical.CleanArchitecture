using ClassifiedAds.Blazor.Modules.Products.Models;
using System.Collections.Generic;

namespace ClassifiedAds.Blazor.Modules.Products.Components
{
    public partial class AuditLogsDialog
    {
        public bool ShowDialog { get; set; }

        public List<ProductAuditLogModel> AuditLogs { get; set; }

        public void Show(List<ProductAuditLogModel> auditLogs)
        {
            AuditLogs = auditLogs;
            ShowDialog = true;
            StateHasChanged();
        }

        public void Close()
        {
            ShowDialog = false;
            StateHasChanged();
        }
    }
}
