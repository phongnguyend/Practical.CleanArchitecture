using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Blazor.Services;
using ClassifiedAds.Blazor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace ClassifiedAds.Blazor.Pages
{
    public class ProductListBase : ComponentBase
    {
        [CascadingParameter]
        Task<AuthenticationState> AuthenticationStateTask { get; set; }

        [Inject]
        public ProductService ProductService { get; set; }

        [Inject]
        public NavigationManager NavManager { get; set; }

        [Inject]
        public ILogger<ProductListBase> Logger { get; set; }

        protected AddProductDialog AddProductDialog { get; set; }

        protected DeleteProductDialog DeleteProductDialog { get; set; }

        public List<Product> Products { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Products = await ProductService.GetProducts();
        }

        public async void AddProductDialog_OnDialogClose()
        {
            Products = await ProductService.GetProducts();
            StateHasChanged();
        }

        protected async Task QuickAddProduct()
        {
            var authenticationState = await AuthenticationStateTask;
            AddProductDialog.Show("Quick Add Product", new Product());
        }

        protected void EditProduct(Product product)
        {
            AddProductDialog.Show("Quick Update Product", product);
        }

        protected void DeleteProduct(Product product)
        {
            DeleteProductDialog.Show(product);
        }

        public async void DeleteProductDialog_OnDialogClose()
        {
            Products = await ProductService.GetProducts();
            StateHasChanged();
        }
    }
}
