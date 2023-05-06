using ClassifiedAds.Blazor.Modules.Core.Components;
using ClassifiedAds.Blazor.Modules.Files.Components;
using ClassifiedAds.Blazor.Modules.Files.Models;
using ClassifiedAds.Blazor.Modules.Files.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClassifiedAds.Blazor.Modules.Files.Pages;

public partial class List
{
    [Inject]
    public NavigationManager NavManager { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    [Inject]
    public ILogger<List> Logger { get; set; }

    [Inject]
    public FileService FileService { get; set; }

    public List<FileEntryModel> Files { get; set; } = new List<FileEntryModel>();

    protected AuditLogsDialog AuditLogsDialog { get; set; }

    protected ConfirmDialog DeleteDialog { get; set; }

    public FileEntryModel DeletingFile { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        Files = await FileService.GetFilesAsync();
    }

    protected async Task Download(FileEntryModel file)
    {
        var token = await FileService.GetAccessToken();
        await JSRuntime.Log(token);
        await JSRuntime.Log(file);
        await JSRuntime.InvokeVoidAsync("interop.downloadFile", FileService.GetDownloadUrl(file.Id), token, file.FileName);
    }

    protected async Task ViewAuditLogs(FileEntryModel file)
    {
        var logs = await FileService.GetAuditLogsAsync(file.Id);
        await JSRuntime.Table(logs);
        AuditLogsDialog.Show(logs);
    }

    protected void DeleteFile(FileEntryModel file)
    {
        DeletingFile = file;
        DeleteDialog.Show();
    }

    public async void ConfirmedDeleteFile()
    {
        Logger.LogWarning($"Deleting File: {DeletingFile.Id}");

        await FileService.DeleteFileAsync(DeletingFile.Id);
        DeleteDialog.Close();

        Logger.LogWarning($"Deleted File: {DeletingFile.Id}");

        Files = await FileService.GetFilesAsync();
        StateHasChanged();
    }
}
