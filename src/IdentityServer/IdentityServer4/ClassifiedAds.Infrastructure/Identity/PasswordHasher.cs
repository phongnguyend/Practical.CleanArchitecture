using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace ClassifiedAds.Infrastructure.Identity;

public class PasswordHasher : IPasswordHasher
{
    public bool VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
    {
        return new PasswordHasher<User>().VerifyHashedPassword(user, hashedPassword, providedPassword) != PasswordVerificationResult.Failed;
    }
}
