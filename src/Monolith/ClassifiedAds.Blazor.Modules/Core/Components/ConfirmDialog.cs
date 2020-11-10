using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace ClassifiedAds.Blazor.Modules.Core.Components
{
    public partial class ConfirmDialog
    {
        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string YesText { get; set; }

        [Parameter]
        public string NoText { get; set; }

        public bool ShowDialog { get; set; }

        [Parameter]
        public EventCallback<bool> ConfirmEventCallback { get; set; }

        [Parameter]
        public RenderFragment Message { get; set; }

        public void Show()
        {
            ShowDialog = true;
            StateHasChanged();
        }

        public void Close()
        {
            ShowDialog = false;
            StateHasChanged();
        }

        protected async Task HandleValidSubmit()
        {
            await ConfirmEventCallback.InvokeAsync(true);
            StateHasChanged();
        }
    }
}
