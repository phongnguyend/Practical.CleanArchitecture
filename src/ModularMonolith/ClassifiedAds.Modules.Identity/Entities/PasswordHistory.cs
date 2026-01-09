using ClassifiedAds.Domain.Entities;
using System;

namespace ClassifiedAds.Modules.Identity.Entities;

public class PasswordHistory : Entity<Guid>
{
    public Guid UserId { get; set; }

    public string PasswordHash { get; set; }

    public virtual User User { get; set; }
}