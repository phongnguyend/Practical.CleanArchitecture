using RazorLight;
using System;

namespace Microsoft.Extensions.DependencyInjection;

public static class HtmlCollectionExtensions
{
    public static IServiceCollection AddHtmlRazorLightEngine(this IServiceCollection services)
    {
        var engine = new RazorLightEngineBuilder()
               .UseFileSystemProject(Environment.CurrentDirectory)
               .UseMemoryCachingProvider()
               .Build();

        services.AddSingleton<IRazorLightEngine>(engine);

        return services;
    }
}
