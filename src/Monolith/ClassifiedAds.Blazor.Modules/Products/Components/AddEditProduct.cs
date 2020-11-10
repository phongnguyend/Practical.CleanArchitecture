using ClassifiedAds.Blazor.Modules.Products.Models;
using ClassifiedAds.Blazor.Modules.Products.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace ClassifiedAds.Blazor.Modules.Products.Components
{
    public partial class AddEditProduct
    {
        [CascadingParameter]
        Task<AuthenticationState> AuthenticationStateTask { get; set; }

        [Inject]
        public ProductService ProductService { get; set; }

        [Inject]
        public NavigationManager NavManager { get; set; }

        [Inject]
        public ILogger<AddEditProduct> Logger { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [Parameter]
        public ProductModel Product { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("interop.focusElementById", "productName");
            }
        }

        protected async Task HandleValidSubmit()
        {
            await (Product.Id == Guid.Empty ? ProductService.CreateProduct(Product) : ProductService.UpdateProduct(Product.Id, Product));
            NavManager.NavigateTo("/products");
        }
    }
}
