using ClassifiedAds.Domain.Entities;

namespace ClassifiedAds.Services.Identity.Entities;

public class PasswordHistory : Entity<Guid>
{
    public Guid UserId { get; set; }

    public string PasswordHash { get; set; }

    public virtual User User { get; set; }
}