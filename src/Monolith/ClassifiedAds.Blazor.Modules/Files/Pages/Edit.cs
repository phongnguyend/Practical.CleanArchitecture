using ClassifiedAds.Blazor.Modules.Files.Components;
using ClassifiedAds.Blazor.Modules.Files.Models;
using ClassifiedAds.Blazor.Modules.Files.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ClassifiedAds.Blazor.Modules.Files.Pages;

public partial class Edit
{
    [Inject]
    public NavigationManager NavManager { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    [Inject]
    public FileService FileService { get; set; }

    [Parameter]
    public Guid Id { get; set; }

    public FileEntryModel File { get; set; } = new FileEntryModel();

    protected AuditLogsDialog AuditLogsDialog { get; set; }

    protected override async Task OnInitializedAsync()
    {
        File = await FileService.GetFileByIdAsync(Id);
    }

    protected async Task ViewAuditLogs()
    {
        var logs = await FileService.GetAuditLogsAsync(File.Id);
        AuditLogsDialog.Show(logs);
    }

    protected async Task HandleValidSubmit()
    {
        await FileService.UpdateFileAsync(File.Id, File);
        NavManager.NavigateTo("/files");
    }

    protected async Task DownloadEmbedding(FileEntryEmbeddingModel embedding)
    {
        using var streamRef = new DotNetStreamReference(stream: new MemoryStream(Encoding.UTF8.GetBytes(embedding.Embedding)));

        await JSRuntime.InvokeVoidAsync("interop.downloadFileFromStream", "Embedding.txt", streamRef);
    }
}
