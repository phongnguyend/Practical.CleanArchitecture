using ClassifiedAds.Blazor.Services;
using ClassifiedAds.Domain.Entities;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace ClassifiedAds.Blazor.Components
{
    public class AddProductDialogBase : ComponentBase
    {
        public bool ShowDialog { get; set; }

        public string Title { get; set; }

        public Product Product { get; set; }

        [Parameter]
        public EventCallback<bool> CloseEventCallback { get; set; }

        [Inject]
        public ProductService ProductService { get; set; }

        public void Show(string title, Product product)
        {
            Title = title;
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
            await (Product.Id == Guid.Empty ? ProductService.CreateProduct(Product): ProductService.UpdateProduct(Product.Id, Product)) ;
            ShowDialog = false;

            await CloseEventCallback.InvokeAsync(true);
            StateHasChanged();
        }
    }
}
