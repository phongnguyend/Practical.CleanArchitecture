using Microsoft.Extensions.Configuration;

namespace ClassifiedAds.Infrastructure.Configuration
{
    public static class HashiCorpVaultConfigurationExtensions
    {
        public static IConfigurationBuilder AddHashiCorpVault(this IConfigurationBuilder builder, HashiCorpVaultOptions options)
        {
            return builder.Add(new HashiCorpVaultConfigurationSource(options));
        }
    }
}
