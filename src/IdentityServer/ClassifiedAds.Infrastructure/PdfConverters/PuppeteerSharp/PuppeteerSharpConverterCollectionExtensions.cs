using ClassifiedAds.CrossCuttingConcerns.PdfConverter;
using ClassifiedAds.Infrastructure.PdfConverters.PuppeteerSharp;
using PuppeteerSharp;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PuppeteerSharpConverterCollectionExtensions
    {
        public static IServiceCollection AddPuppeteerSharpPdfConverter(this IServiceCollection services)
        {
            var browserFetcher = new BrowserFetcher();
            browserFetcher.DownloadAsync().GetAwaiter().GetResult();

            services.AddSingleton<IPdfConverter, PuppeteerSharpConverter>();

            return services;
        }
    }
}
