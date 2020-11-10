using ClassifiedAds.Blazor.Modules.Core.Components;
using ClassifiedAds.Blazor.Modules.Files.Components;
using ClassifiedAds.Blazor.Modules.Files.Models;
using ClassifiedAds.Blazor.Modules.Files.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassifiedAds.Blazor.Modules.Files.Pages
{
    public partial class List
    {
        [Inject]
        public NavigationManager NavManager { get; set; }

        [Inject]
        public FileService FileService { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        public List<FileEntryModel> Files { get; set; } = new List<FileEntryModel>();

        protected AuditLogsDialog AuditLogsDialog { get; set; }

        protected ConfirmDialog DeleteDialog { get; set; }

        public FileEntryModel DeletingFile { get; private set; }

        protected override async Task OnInitializedAsync()
        {
            Files = await FileService.GetFiles();
        }
        protected async Task Download(FileEntryModel file)
        {
            var token = await FileService.GetAccessToken();
            JSRuntime.Log(token);
            JSRuntime.Log(file);
            await JSRuntime.InvokeVoidAsync("interop.downloadFile", FileService.GetDownloadUrl(file.Id), token, file.FileName);
        }


        protected async Task ViewAuditLogs(FileEntryModel file)
        {
            var logs = await FileService.GetAuditLogs(file.Id);
            JSRuntime.Table(logs);
            AuditLogsDialog.Show(logs);
        }

        protected void DeleteFile(FileEntryModel file)
        {
            DeletingFile = file;
            DeleteDialog.Show();
        }

        public async void ConfirmedDeleteFile()
        {
            await FileService.DeleteFile(DeletingFile.Id);
            DeleteDialog.Close();
            Files = await FileService.GetFiles();
            StateHasChanged();
        }
    }
}
