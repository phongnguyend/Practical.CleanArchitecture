using IdentityServer4.EntityFramework.Entities;

namespace ClassifiedAds.IdentityServer.Models.ApiResourceModels
{
    public class ApiResourcePropertyModel
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public ApiResourceModel ApiResource { get; set; }

        public static ApiResourcePropertyModel FromEntity(ApiResourceProperty entity)
        {
            return new ApiResourcePropertyModel
            {
                Id = entity.Id,
                Key = entity.Key,
                Value = entity.Value,
                ApiResource = new ApiResourceModel
                {
                    Id = entity.ApiResource.Id,
                    Name = entity.ApiResource.Name,
                },
            };
        }
    }
}
