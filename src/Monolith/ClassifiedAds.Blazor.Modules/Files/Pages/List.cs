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

    public List<FileEntryVectorSearchResultModel> VectorSearchResults { get; set; } = new List<FileEntryVectorSearchResultModel>();

    protected AuditLogsDialog AuditLogsDialog { get; set; }

    protected ConfirmDialog DeleteDialog { get; set; }

    public FileEntryModel DeletingFile { get; private set; }

    protected string SearchText { get; set; }

    public bool ShowVectorSearch { get; set; }

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

    protected async Task VectorSearch()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            return;
        }

        VectorSearchResults = await FileService.VectorSearchFilesAsync(SearchText);
        ShowVectorSearch = true;
    }

    protected async Task ClearSearch()
    {
        SearchText = string.Empty;
        Files = await FileService.GetFilesAsync();
        ShowVectorSearch = false;
    }
}
