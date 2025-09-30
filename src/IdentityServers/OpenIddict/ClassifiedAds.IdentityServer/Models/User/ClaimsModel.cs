using System.Collections.Generic;
using System.Linq;

namespace ClassifiedAds.IdentityServer.Models.User;

public class ClaimsModel : ClaimModel
{
    public List<ClaimModel> Claims { get; set; }

    public static ClaimsModel FromEntity(Domain.Entities.User user)
    {
        return new ClaimsModel
        {
            User = user,
            Claims = user.Claims?.Select(x => FromEntity(x))?.ToList(),
        };
    }
}
