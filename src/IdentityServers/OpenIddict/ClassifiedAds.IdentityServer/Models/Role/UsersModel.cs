using System.Collections.Generic;

namespace ClassifiedAds.IdentityServer.Models.Role;

public class UsersModel
{
    public Domain.Entities.Role Role { get; set; }

    public List<Domain.Entities.User> Users { get; set; }
}
