using IdentityServer4.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassifiedAds.IdentityServer.Models.ApiResourceModels
{
    public class ScopesModel : ScopeModel
    {
        public List<ScopeModel> Scopes { get; set; }

        public void UpdateEntity(ApiScope entity)
        {

        }

        public static ScopesModel FromEntity(ApiResource apiResource)
        {
            return new ScopesModel
            {
                ApiResourceId = apiResource.Id,
                ApiResourceName = apiResource.Name,
                Scopes = apiResource.Scopes?.Select(x => FromEntity(x))?.ToList(),
            };
        }
    }
}
