using System;

namespace ClassifiedAds.IdentityServer.Models.User;

public class RoleModel
{
    public Guid RoleId { get; set; }

    public Guid UserId { get; set; }

    public Domain.Entities.User User { get; set; }

    public Domain.Entities.Role Role { get; set; }
}
