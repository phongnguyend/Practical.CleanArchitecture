using ClassifiedAds.Blazor.Modules.Products.Components;
using ClassifiedAds.Blazor.Modules.Products.Models;
using ClassifiedAds.Blazor.Modules.Products.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ClassifiedAds.Blazor.Modules.Products.Pages;

public partial class Detail
{
    [Inject]
    public ProductService ProductService { get; set; }

    [Inject]
    public NavigationManager NavManager { get; set; }

    [Inject]
    public ILogger<Detail> Logger { get; set; }

    [Parameter]
    public Guid ProductId { get; set; }

    public ProductModel Product { get; set; } = new ProductModel();

    protected AuditLogsDialog AuditLogsDialog { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Product = await ProductService.GetProductByIdAsync(ProductId);
    }

    protected async Task ViewAuditLogs()
    {
        var logs = await ProductService.GetAuditLogsAsync(Product.Id);
        AuditLogsDialog.Show(logs);
    }
}
