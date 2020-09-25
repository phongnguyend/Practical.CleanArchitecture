using ClassifiedAds.Modules.Identity.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ClassifiedAds.IdentityServer.Models.RoleModels
{
    public class ClaimsModel : ClaimModel
    {
        public List<ClaimModel> Claims { get; set; }

        public static ClaimsModel FromEntity(Role role)
        {
            return new ClaimsModel
            {
                Role = role,
                Claims = role.Claims?.Select(x => FromEntity(x))?.ToList(),
            };
        }
    }
}
