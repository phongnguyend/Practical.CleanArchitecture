using ClassifiedAds.Infrastructure.Pdf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;

namespace Microsoft.Extensions.DependencyInjection;

public static class HtmlToPdfServiceCollectionExtensions
{
    public static IServiceCollection AddHtmlToPdfClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<HtmlToPdfOptions>()
            .Bind(configuration)
            .Validate(options => Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
                && string.IsNullOrEmpty(uri.Query) && string.IsNullOrEmpty(uri.Fragment),
                "HtmlToPdf:BaseUrl must be an absolute HTTP(S) URL without a query or fragment.")
            .Validate(options => options.TimeoutSeconds > 0 && options.TimeoutSeconds <= 600,
                "HtmlToPdf:TimeoutSeconds must be between 1 and 600.")
            .ValidateOnStart();

        services.AddHttpClient<HtmlToPdfClient>((provider, client) =>
        {
            var options = provider.GetRequiredService<IOptions<HtmlToPdfOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/') + "/");
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });

        return services;
    }
}
