using ClassifiedAds.Application.Products.DTOs;
using ClassifiedAds.CrossCuttingConcerns.Html;
using ClassifiedAds.Infrastructure.Html;
using RazorLight;
using System;

namespace Microsoft.Extensions.DependencyInjection;

public static class HtmlCollectionExtensions
{
    public static IServiceCollection AddHtmlWriters(this IServiceCollection services)
    {
        var engine = new RazorLightEngineBuilder()
               .UseFileSystemProject(Environment.CurrentDirectory)
               .UseMemoryCachingProvider()
               .Build();

        services.AddSingleton<IRazorLightEngine>(engine);
        services.AddSingleton<IHtmlWriter<ExportProductsToHtml>, ExportProductsToHtmlHandler>();

        return services;
    }
}
