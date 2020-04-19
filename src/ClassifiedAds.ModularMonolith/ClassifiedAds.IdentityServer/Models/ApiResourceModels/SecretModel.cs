using IdentityServer4.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassifiedAds.IdentityServer.Models.ApiResourceModels
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
        public int ApiResourceId { get; set; }
        public string ApiResourceName { get; set; }
        public ApiResourceModel ApiResource { get; set; }

        public static SecretModel FromEntity(ApiSecret secret)
        {
            return new SecretModel
            {
                Id = secret.Id,
                Description = secret.Description,
                Value = secret.Value,
                Expiration = secret.Expiration,
                Type = secret.Type,
                Created = secret.Created,
                ApiResourceId = secret.ApiResource.Id,
                ApiResourceName = secret.ApiResource.Name,
            };
        }
    }
}
