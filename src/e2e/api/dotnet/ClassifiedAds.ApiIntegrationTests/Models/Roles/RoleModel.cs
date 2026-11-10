using System;

namespace ClassifiedAds.ApiIntegrationTests.Models.Roles;

public class RoleModel
{
    public Guid Id { get; set; }

    public virtual string Name { get; set; }

    public virtual string NormalizedName { get; set; }

    public virtual string ConcurrencyStamp { get; set; }
}
