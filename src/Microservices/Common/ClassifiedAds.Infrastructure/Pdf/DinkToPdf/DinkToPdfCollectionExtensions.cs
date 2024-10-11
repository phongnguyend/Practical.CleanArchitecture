using DinkToPdf;
using DinkToPdf.Contracts;

namespace Microsoft.Extensions.DependencyInjection;

public static class DinkToPdfCollectionExtensions
{
    public static IServiceCollection AddDinkToPdfConverter(this IServiceCollection services)
    {
        services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
        return services;
    }
}
