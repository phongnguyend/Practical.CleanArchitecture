using ClassifiedAds.Modules.Identity.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ClassifiedAds.IdentityServer.Models.UserModels
{
    public class ClaimsModel : ClaimModel
    {
        public List<ClaimModel> Claims { get; set; }

        public static ClaimsModel FromEntity(User user)
        {
            return new ClaimsModel
            {
                User = user,
                Claims = user.Claims?.Select(x => FromEntity(x))?.ToList(),
            };
        }
    }
}
