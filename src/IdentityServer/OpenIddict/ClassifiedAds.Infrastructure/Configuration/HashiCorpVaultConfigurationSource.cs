using Microsoft.Extensions.Configuration;

namespace ClassifiedAds.Infrastructure.Configuration;

public class HashiCorpVaultConfigurationSource : IConfigurationSource
{
    private readonly HashiCorpVaultOptions _options;

    public HashiCorpVaultConfigurationSource(HashiCorpVaultOptions options)
    {
        _options = options;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new HashiCorpVaultConfigurationProvider(_options);
    }
}
