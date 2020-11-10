using ClassifiedAds.Blazor.Modules.Files.Models;
using ClassifiedAds.Blazor.Modules.Files.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace ClassifiedAds.Blazor.Modules.Files.Pages
{
    public partial class Upload
    {
        [Inject]
        public NavigationManager NavManager { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [Inject]
        public FileService FileService { get; set; }

        public FileEntryModel File { get; set; } = new FileEntryModel();

        protected async Task HandleValidSubmit()
        {
            var dotNetObj = DotNetObjectReference.Create(this);
            await JSRuntime.InvokeVoidAsync("interop.uploadFile", FileService.GetUploadUrl(), await FileService.GetAccessToken(), File, dotNetObj);
            
        }

        [JSInvokable]
        public void Uploaded(string id)
        {
            NavManager.NavigateTo($"/files/edit/{id}");
        }
    }
}
