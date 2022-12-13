using IdentityServer4.EntityFramework.Entities;

namespace ClassifiedAds.IdentityServer.Models.IdentityResourceModels
{
    public class IdentityResourcePropertyModel
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public IdentityResourceModel IdentityResource { get; set; }

        public static IdentityResourcePropertyModel FromEntity(IdentityResourceProperty entity)
        {
            return new IdentityResourcePropertyModel
            {
                Id = entity.Id,
                Key = entity.Key,
                Value = entity.Value,
                IdentityResource = new IdentityResourceModel
                {
                    Id = entity.IdentityResource.Id,
                    Name = entity.IdentityResource.Name,
                },
            };
        }
    }
}
