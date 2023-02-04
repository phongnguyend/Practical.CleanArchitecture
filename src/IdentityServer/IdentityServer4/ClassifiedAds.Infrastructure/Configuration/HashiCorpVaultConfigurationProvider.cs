using Microsoft.Extensions.Configuration;
using System.Linq;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.AuthMethods.UserPass;

namespace ClassifiedAds.Infrastructure.Configuration
{
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
}