using ClassifiedAds.Application.Products.DTOs;
using ClassifiedAds.CrossCuttingConcerns.Pdf;
using ClassifiedAds.Infrastructure.PdfConverters.PuppeteerSharp;
using PuppeteerSharp;

namespace Microsoft.Extensions.DependencyInjection;

public static class PuppeteerSharpCollectionExtensions
{
    public static IServiceCollection AddPuppeteerSharpPdfWriter(this IServiceCollection services)
    {
        var browserFetcher = new BrowserFetcher();
        browserFetcher.DownloadAsync().GetAwaiter().GetResult();

        services.AddSingleton<IPdfWriter<ExportProductsToPdf>, ExportProductsToPdfHandler>();

        return services;
    }
}
