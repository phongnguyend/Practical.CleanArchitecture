using ClassifiedAds.Modules.Identity.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ClassifiedAds.Modules.Identity.DTOs.Users
{
    public static class UserDTOMappingConfiguration
    {
        public static IEnumerable<UserDTO> ToDTOs(this IEnumerable<User> entities)
        {
            return entities.Select(x => x.ToDTO());
        }

        public static UserDTO ToDTO(this User entity)
        {
            return new UserDTO
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
}
