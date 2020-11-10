using ClassifiedAds.Blazor.Modules.Products.Models;
using ClassifiedAds.Blazor.Modules.Products.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ClassifiedAds.Blazor.Modules.Products.Pages
{
    public partial class Add
    {
        [CascadingParameter]
        Task<AuthenticationState> AuthenticationStateTask { get; set; }

        [Inject]
        public ProductService ProductService { get; set; }

        [Inject]
        public NavigationManager NavManager { get; set; }

        [Inject]
        public ILogger<Add> Logger { get; set; }

        public ProductModel Product { get; set; }

        protected override void OnInitialized()
        {
            Product = new ProductModel { };
        }
    }
}
