using ClassifiedAds.Application.Products.DTOs;
using ClassifiedAds.CrossCuttingConcerns.Pdf;
using ClassifiedAds.Infrastructure.PdfConverters.DinkToPdf;
using DinkToPdf;
using DinkToPdf.Contracts;

namespace Microsoft.Extensions.DependencyInjection;

public static class DinkToPdfCollectionExtensions
{
    public static IServiceCollection AddDinkToPdfWriters(this IServiceCollection services)
    {
        services.AddSingleton<IConverter>(new SynchronizedConverter(new PdfTools()));
        services.AddSingleton<IPdfWriter<ExportProductsToPdf>, ExportProductsToPdfHandler>();

        return services;
    }
}
