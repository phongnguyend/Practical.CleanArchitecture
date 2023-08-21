using Microsoft.Extensions.Configuration;
using System.Linq;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.AuthMethods.UserPass;

namespace ClassifiedAds.Infrastructure.Configuration;

internal class HashiCorpVaultConfigurationProvider : ConfigurationProvider
{
    private readonly HashiCorpVaultOptions _options;

    public HashiCorpVaultConfigurationProvider(HashiCorpVaultOptions options)
    {
        _options = options;
    }

    public override void Load()
    {
        IAuthMethodInfo authMethod = null;

        if (_options.AuthMethod == "Token")
        {
            authMethod = new TokenAuthMethodInfo(_options.Auth.Token.Token);
        }
        else if (_options.AuthMethod == "UserPass")
        {
            authMethod = new UserPassAuthMethodInfo(_options.Auth.UserPass.UserName, _options.Auth.UserPass.Password);
        }

        var vaultClientSettings = new VaultClientSettings(_options.Address, authMethod);
        var client = new VaultClient(vaultClientSettings);
        var secrets = client.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: _options.SecretPath, mountPoint: _options.SecretEnginePath)
            .GetAwaiter().GetResult();
        Data = secrets.Data.Data.ToDictionary(x => x.Key, x => x.Value.ToString());
    }
}

public class HashiCorpVaultOptions
{
    public bool IsEnabled { get; set; }

    public string Address { get; set; }

    public string SecretEnginePath { get; set; }

    public string SecretPath { get; set; }

    public string AuthMethod { get; set; }

    public HashiCorpVaultAuthOptions Auth { get; set; }
}

public class HashiCorpVaultAuthOptions
{
    public HashiCorpVaultTokenAuthOptions Token { get; set; }

    public HashiCorpVaultUserPassAuthOptions UserPass { get; set; }
}

public class HashiCorpVaultUserPassAuthOptions
{
    public string UserName { get; set; }

    public string Password { get; set; }
}

public class HashiCorpVaultTokenAuthOptions
{
    public string Token { get; set; }
}

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

public static class HashiCorpVaultConfigurationExtensions
{
    public static IConfigurationBuilder AddHashiCorpVault(this IConfigurationBuilder builder, HashiCorpVaultOptions options)
    {
        return builder.Add(new HashiCorpVaultConfigurationSource(options));
    }
}