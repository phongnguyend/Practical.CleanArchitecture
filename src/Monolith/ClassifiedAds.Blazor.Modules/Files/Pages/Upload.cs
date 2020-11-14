using ClassifiedAds.Blazor.Modules.Files.Models;
using ClassifiedAds.Blazor.Modules.Files.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Linq;
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

        public EditContext EditContext { get; set; }

        protected override void OnInitialized()
        {
            EditContext = new EditContext(File);
            EditContext.SetFieldCssClassProvider(new MyFieldClassProvider());
        }

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

        private async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            var file = e.File;
            var buffer = new byte[file.Size];
            await file.OpenReadStream().ReadAsync(buffer);

            File.FileName = file.Name;
            var res = await FileService.UploadFile(File, buffer);

            NavManager.NavigateTo($"/files/edit/{res.Id}");
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
}
