using ClassifiedAds.Blazor.Modules.Products.Components;
using ClassifiedAds.Blazor.Modules.Products.Models;
using ClassifiedAds.Blazor.Modules.Products.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System;
using System.IO;
using System.Text;
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

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    [Parameter]
    public Guid ProductId { get; set; }

    public ProductModel Product { get; set; } = new ProductModel();

    protected AuditLogsDialog AuditLogsDialog { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Product = await ProductService.GetProductByIdAsync(ProductId);
    }

    protected override async Task OnParametersSetAsync()
    {
        Product = await ProductService.GetProductByIdAsync(ProductId);
    }

    protected async Task ViewAuditLogs()
    {
        var logs = await ProductService.GetAuditLogsAsync(Product.Id);
        AuditLogsDialog.Show(logs);
    }

    protected async Task DownloadProductEmbedding()
    {
        using var streamRef = new DotNetStreamReference(stream: new MemoryStream(Encoding.UTF8.GetBytes(Product.ProductEmbedding.Embedding)));

        await JSRuntime.InvokeVoidAsync("interop.downloadFileFromStream", "Embedding.txt", streamRef);
    }
}
