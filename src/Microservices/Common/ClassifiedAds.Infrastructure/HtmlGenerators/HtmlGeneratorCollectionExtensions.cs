using ClassifiedAds.CrossCuttingConcerns.HtmlGenerator;
using ClassifiedAds.Infrastructure.HtmlGenerators;
using RazorLight;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HtmlGeneratorCollectionExtensions
    {
        public static IServiceCollection AddHtmlGenerator(this IServiceCollection services)
        {
            var engine = new RazorLightEngineBuilder()
                   .UseFileSystemProject(Environment.CurrentDirectory)
                   .UseMemoryCachingProvider()
                   .Build();

            services.AddSingleton<IRazorLightEngine>(engine);
            services.AddSingleton<IHtmlGenerator, HtmlGenerator>();

            return services;
        }
    }
}
