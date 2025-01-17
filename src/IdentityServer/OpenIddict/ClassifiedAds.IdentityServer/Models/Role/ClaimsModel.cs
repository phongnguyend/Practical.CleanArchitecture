using System.Collections.Generic;
using System.Linq;

namespace ClassifiedAds.IdentityServer.Models.Role;

public class ClaimsModel : ClaimModel
{
    public List<ClaimModel> Claims { get; set; }

    public static ClaimsModel FromEntity(Domain.Entities.Role role)
    {
        return new ClaimsModel
        {
            Role = role,
            Claims = role.Claims?.Select(x => FromEntity(x))?.ToList(),
        };
    }
}
