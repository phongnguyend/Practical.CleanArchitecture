using System.Collections.Generic;

namespace ClassifiedAds.IdentityServer.Models.User;

public class RolesModel
{
    public Domain.Entities.User User { get; set; }

    public RoleModel Role { get; set; }

    public List<Domain.Entities.Role> Roles { get; set; }

    public List<RoleModel> UserRoles { get; set; }
}
