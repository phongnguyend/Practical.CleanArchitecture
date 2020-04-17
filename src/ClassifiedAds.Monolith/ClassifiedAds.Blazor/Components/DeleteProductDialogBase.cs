using ClassifiedAds.Blazor.Services;
using ClassifiedAds.Domain.Entities;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace ClassifiedAds.Blazor.Components
{
    public class DeleteProductDialogBase : ComponentBase
    {
        public bool ShowDialog { get; set; }

        public Product Product { get; set; }

        [Parameter]
        public EventCallback<bool> CloseEventCallback { get; set; }

        [Inject]
        public ProductService ProductService { get; set; }

        public void Show(Product product)
        {
            Product = product;
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
            await ProductService.DeleteProduct(Product.Id);
            ShowDialog = false;

            await CloseEventCallback.InvokeAsync(true);
            StateHasChanged();
        }
    }
}
