using IdentityServer4.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassifiedAds.IdentityServer.Models.IdentityResourceModels
{
    public class IdentityResourceModel
    {
        public int Id { get; set; }
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public bool Emphasize { get; set; }
        public bool ShowInDiscoveryDocument { get; set; }
        public List<string> UserClaims { get; set; }
        public string UserClaimsItems { get; set; }
        public List<IdentityResourcePropertyModel> Properties { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public bool NonEditable { get; set; }

        public void UpdateEntity(IdentityResource entity)
        {
            entity.Enabled = Enabled;
            entity.Name = Name;
            entity.DisplayName = DisplayName;
            entity.Description = Description;
            entity.Required = Required;
            entity.Emphasize = Emphasize;
            entity.ShowInDiscoveryDocument = ShowInDiscoveryDocument;
            entity.Created = Created;
            entity.Updated = Updated;
            entity.NonEditable = NonEditable;
        }
    }

    public static class IdentityResourceMappingExtension
    {
        public static IdentityResourceModel ToModel(this IdentityResource entity)
        {
            return new IdentityResourceModel
            {
                Id = entity.Id,
                Enabled = entity.Enabled,
                Name = entity.Name,
                DisplayName = entity.DisplayName,
                Description = entity.Description,
                Required = entity.Required,
                Emphasize = entity.Emphasize,
                ShowInDiscoveryDocument = entity.ShowInDiscoveryDocument,
                Created = entity.Created,
                Updated = entity.Updated,
                NonEditable = entity.NonEditable,
                UserClaims = entity.UserClaims?.Select(x => x.Type)?.ToList(),
            };
        }

        public static IEnumerable<IdentityResourceModel> ToModels(this IEnumerable<IdentityResource> entities)
        {
            return entities?.Select(x => x.ToModel());
        }
    }
}
