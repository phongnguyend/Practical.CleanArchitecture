using IdentityServer4.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassifiedAds.IdentityServer.Models.ApiResourceModels
{
    public class ScopeModel
    {
        public int Id { get; set; }
        public string Scope { get; set; }
        public int ApiResourceId { get; set; }
        public ApiResource ApiResource { get; set; }
        public string ApiResourceName { get; set; }

        public static ScopeModel FromEntity(ApiResourceScope apiScope)
        {
            return new ScopeModel
            {
                Id = apiScope.Id,
                Scope = apiScope.Scope,
                ApiResourceId = apiScope.ApiResourceId,
                ApiResourceName = apiScope.ApiResource.Name,
                ApiResource = apiScope.ApiResource,
            };
        }

        public void UpdateEntity(ApiResourceScope entity)
        {
            entity.Scope = Scope;
        }
    }
}
