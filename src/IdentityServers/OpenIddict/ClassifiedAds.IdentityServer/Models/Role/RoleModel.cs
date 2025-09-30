using System;

namespace ClassifiedAds.IdentityServer.Models.Role;

public class RoleModel
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public static RoleModel FromEntity(Domain.Entities.Role role)
    {
        return new RoleModel { Id = role.Id, Name = role.Name };
    }
}
