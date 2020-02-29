using IdentityServer4.EntityFramework.Entities;
using System;

namespace ClassifiedAds.IdentityServer.Models.ClientModels
{
    public class SecretModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public DateTime? Expiration { get; set; }
        public string Type { get; set; }
        public string HashType { get; set; }
        public DateTime Created { get; set; }
        public ClientModel Client { get; set; }

        public static SecretModel FromEntity(ClientSecret secret)
        {
            return new SecretModel
            {
                Id = secret.Id,
                Description = secret.Description,
                Value = secret.Value,
                Expiration = secret.Expiration,
                Type = secret.Type,
                Created = secret.Created,
                Client = ClientModel.FromEntity(secret.Client),
            };
        }
    }
}
