using ClassifiedAds.Blazor.Modules.Products.Models;
using ClassifiedAds.Blazor.Modules.Products.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ClassifiedAds.Blazor.Modules.Products.Pages;

public partial class Edit
{
    [CascadingParameter]
    Task<AuthenticationState> AuthenticationStateTask { get; set; }

    [Inject]
    public ProductService ProductService { get; set; }

    [Inject]
    public NavigationManager NavManager { get; set; }

    [Inject]
    public ILogger<Edit> Logger { get; set; }

    [Parameter]
    public Guid ProductId { get; set; }

    public ProductModel Product { get; set; } = new ProductModel();

    protected override async Task OnInitializedAsync()
    {
        Product = await ProductService.GetProductByIdAsync(ProductId);
    }
}
