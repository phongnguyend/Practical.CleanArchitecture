using ClassifiedAds.Domain.Entities;

namespace ClassifiedAds.Domain.Identity;

public interface IPasswordHasher
{
    bool VerifyHashedPassword(User user, string hashedPassword, string providedPassword);
}
