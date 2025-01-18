using ClassifiedAds.Domain.Entities;
using System;

namespace ClassifiedAds.Modules.Identity.Entities;

public class UserLogin : Entity<Guid>, IAggregateRoot
{
    public Guid UserId { get; set; }

    public string LoginProvider { get; set; }

    public string ProviderKey { get; set; }

    public string ProviderDisplayName { get; set; }

    public User User { get; set; }
}
