using ClassifiedAds.Blazor.Modules.Files.Models;
using ClassifiedAds.Blazor.Modules.Files.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ClassifiedAds.Blazor.Modules.Files.Pages;

public partial class Upload
{
    [Inject]
    public NavigationManager NavManager { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    [Inject]
    public ILogger<Upload> Logger { get; set; }

    [Inject]
    public FileService FileService { get; set; }

    public FileEntryModel File { get; set; } = new FileEntryModel();

    public IBrowserFile BrowserFile { get; set; }

    public EditContext EditContext { get; set; }

    protected override void OnInitialized()
    {
        EditContext = new EditContext(File);
        EditContext.SetFieldCssClassProvider(new MyFieldClassProvider());
    }

    protected async Task HandleValidSubmit()
    {
        if (BrowserFile != null)
        {
            Logger.LogWarning("Upload using InputFile + HttpClient.");

            using var memoryStream = new MemoryStream();
            await BrowserFile.OpenReadStream().CopyToAsync(memoryStream);

            File.FileName = BrowserFile.Name;
            var res = await FileService.UploadFileAsync(File, memoryStream.ToArray());

            NavManager.NavigateTo($"/files/edit/{res.Id}");
        }
        else
        {
            Logger.LogWarning("Upload using JavaScript Interop.");

            var dotNetObj = DotNetObjectReference.Create(this);
            await JSRuntime.InvokeVoidAsync("interop.uploadFile", FileService.GetUploadUrl(), await FileService.GetAccessToken(), File, dotNetObj);
        }
    }

    [JSInvokable]
    public void Uploaded(string id)
    {
        NavManager.NavigateTo($"/files/edit/{id}");
    }

    private void OnInputFileChange(InputFileChangeEventArgs e)
    {
        BrowserFile = e.File;
    }
}

public class MyFieldClassProvider : FieldCssClassProvider
{
    public override string GetFieldCssClass(EditContext editContext,
        in FieldIdentifier fieldIdentifier)
    {
        var isValid = !editContext.GetValidationMessages(fieldIdentifier).Any();

        return isValid ? "is-valid" : "is-invalid";
    }
}
