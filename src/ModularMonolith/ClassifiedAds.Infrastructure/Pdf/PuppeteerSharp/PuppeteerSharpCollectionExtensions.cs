using PuppeteerSharp;

namespace Microsoft.Extensions.DependencyInjection;

public static class PuppeteerSharpCollectionExtensions
{
    public static IServiceCollection AddPuppeteerSharpPdfConverter(this IServiceCollection services)
    {
        var browserFetcher = new BrowserFetcher();
        browserFetcher.DownloadAsync().GetAwaiter().GetResult();
        return services;
    }
}
