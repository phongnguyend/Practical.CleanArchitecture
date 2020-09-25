using IdentityServer4.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassifiedAds.IdentityServer.Models.ApiResourceModels
{
    public class ApiResourceModel
    {
        public int Id { get; set; }
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public List<ApiSecret> Secrets { get; set; }
        public List<ApiScope> Scopes { get; set; }
        public List<ApiResourceClaim> UserClaims { get; set; }
        public string UserClaimsItems { get; set; }
        public List<ApiResourceProperty> Properties { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? LastAccessed { get; set; }
        public bool NonEditable { get; set; }

        public static ApiResourceModel FromEntity(ApiResource apiResource)
        {
            return new ApiResourceModel
            {
                Id = apiResource.Id,
                Enabled = apiResource.Enabled,
                Name = apiResource.Name,
                DisplayName = apiResource.DisplayName,
                Description = apiResource.Description,
                Secrets = apiResource.Secrets,
                Scopes = apiResource.Scopes,
                UserClaims = apiResource.UserClaims,
                Properties = apiResource.Properties,
                Created = apiResource.Created,
                Updated = apiResource.Updated,
                LastAccessed = apiResource.LastAccessed,
                NonEditable = apiResource.NonEditable,
            };
        }

        public void UpdateEntity(ApiResource entity)
        {
            entity.Enabled = Enabled;
            entity.Name = Name;
            entity.DisplayName = DisplayName;
            entity.Description = Description;
            entity.NonEditable = NonEditable;
        }
    }
}
