using IdentityServer4.EntityFramework.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ClassifiedAds.IdentityServer.Models.ApiResourceModels
{
    public class PropertiesModel
    {
        public int ApiResourceId { get; set; }
        public string ApiResourceName { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public List<ApiResourcePropertyModel> Properties { get; set; }

        public static PropertiesModel FromEntity(ApiResource apiResource)
        {
            return new PropertiesModel
            {
                ApiResourceId = apiResource.Id,
                ApiResourceName = apiResource.Name,
                Properties = apiResource.Properties?.Select(x => new ApiResourcePropertyModel
                {
                    Id = x.Id,
                    Key = x.Key,
                    Value = x.Value,
                })?.ToList(),
            };
        }
    }
}
