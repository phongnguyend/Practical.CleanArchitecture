using IdentityServer4.Models;
using System.Collections.Generic;
using System.Linq;

namespace ClassifiedAds.IdentityServer.Models.ClientModels
{
    public class SecretsModel : SecretModel
    {
        public List<SecretModel> Secrets { get; set; }
        public List<string> TypeList { get; } = new List<string>
        {
            "SharedSecret",
            "X509Thumbprint",
            "X509Name",
            "X509CertificateBase64",
        };
        public List<string> HashTypes { get; } = new List<string>
        {
            "Sha256",
            "Sha512",
        };

        public void HashSecret()
        {
            if (Type != "SharedSecret")
            {
                return;
            }

            if (HashType == "Sha256")
            {
                Value = Value.Sha256();
            }
            else if (HashType == "Sha512")
            {
                Value = Value.Sha512();
            }
        }

        public void UpdateEntity(IdentityServer4.EntityFramework.Entities.Secret entity)
        {
            entity.Description = Description;
            entity.Value = Value;
            entity.Expiration = Expiration;
            entity.Type = Type;
        }

        public static SecretsModel FromEntity(IdentityServer4.EntityFramework.Entities.Client client)
        {
            return new SecretsModel
            {
                Client = ClientModel.FromEntity(client),
                Secrets = client.ClientSecrets?.Select(x => FromEntity(x))?.ToList(),
            };
        }
    }
}
