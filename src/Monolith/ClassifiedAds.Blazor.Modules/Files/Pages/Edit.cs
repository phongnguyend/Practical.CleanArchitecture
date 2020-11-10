using ClassifiedAds.Blazor.Modules.Files.Components;
using ClassifiedAds.Blazor.Modules.Files.Models;
using ClassifiedAds.Blazor.Modules.Files.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace ClassifiedAds.Blazor.Modules.Files.Pages
{
    public partial class Edit
    {
        [Inject]
        public NavigationManager NavManager { get; set; }

        [Inject]
        public FileService FileService { get; set; }

        [Parameter]
        public Guid Id { get; set; }

        public FileEntryModel File { get; set; } = new FileEntryModel();

        protected AuditLogsDialog AuditLogsDialog { get; set; }

        protected override async Task OnInitializedAsync()
        {
            File = await FileService.GetFileById(Id);
        }

        protected async Task ViewAuditLogs()
        {
            var logs = await FileService.GetAuditLogs(File.Id);
            AuditLogsDialog.Show(logs);
        }

        protected async Task HandleValidSubmit()
        {
            await FileService.UpdateFile(File.Id, File);
            NavManager.NavigateTo("/files");
        }
    }
}
