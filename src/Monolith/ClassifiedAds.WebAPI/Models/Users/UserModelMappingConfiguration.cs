using ClassifiedAds.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ClassifiedAds.WebAPI.Models.Users;

public static class UserModelMappingConfiguration
{
    public static IEnumerable<UserModel> ToModels(this IEnumerable<User> entities)
    {
        return entities.Select(x => x.ToModel());
    }

    public static UserModel ToModel(this User entity)
    {
        if (entity == null)
        {
            return null;
        }

        return new UserModel
        {
            Id = entity.Id,
            UserName = entity.UserName,
            NormalizedUserName = entity.NormalizedUserName,
            Email = entity.Email,
            NormalizedEmail = entity.NormalizedEmail,
            EmailConfirmed = entity.EmailConfirmed,
            PhoneNumber = entity.PhoneNumber,
            PhoneNumberConfirmed = entity.PhoneNumberConfirmed,
            TwoFactorEnabled = entity.TwoFactorEnabled,
            ConcurrencyStamp = entity.ConcurrencyStamp,
            SecurityStamp = entity.SecurityStamp,
            LockoutEnabled = entity.LockoutEnabled,
            LockoutEnd = entity.LockoutEnd,
            AccessFailedCount = entity.AccessFailedCount,
        };
    }
}
