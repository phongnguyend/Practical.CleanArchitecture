using IdentityServer4.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassifiedAds.IdentityServer.Models.ApiScopeModels
{
    public class ApiScopeModel
    {
        public int Id { get; set; }
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public bool Emphasize { get; set; }
        public bool ShowInDiscoveryDocument { get; set; }
        public List<ApiScopeClaim> UserClaims { get; set; }
        public string UserClaimsItems { get; set; }
        public List<ApiScopeProperty> Properties { get; set; }

        public static ApiScopeModel FromEntity(ApiScope apiScope)
        {
            return new ApiScopeModel
            {
                Id = apiScope.Id,
                Enabled = apiScope.Enabled,
                Name = apiScope.Name,
                DisplayName = apiScope.DisplayName,
                Description = apiScope.Description,
                Required = apiScope.Required,
                Emphasize = apiScope.Emphasize,
                ShowInDiscoveryDocument = apiScope.ShowInDiscoveryDocument,
                UserClaims = apiScope.UserClaims,
                Properties = apiScope.Properties,
            };
        }

        public void UpdateEntity(ApiScope entity)
        {
            entity.Name = Name;
            entity.DisplayName = DisplayName;
            entity.Description = Description;
            entity.Required = Required;
            entity.Emphasize = Emphasize;
            entity.ShowInDiscoveryDocument = ShowInDiscoveryDocument;
        }
    }
}
