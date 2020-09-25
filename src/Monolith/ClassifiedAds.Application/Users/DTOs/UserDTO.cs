using System;
using System.Collections.Generic;

namespace ClassifiedAds.Application.Users.DTOs
{
    public class UserDTO
    {
        public string UserName { get; set; }

        public string NormalizedUserName { get; set; }

        public string Email { get; set; }

        public string NormalizedEmail { get; set; }

        public bool EmailConfirmed { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public string ConcurrencyStamp { get; set; }

        public string SecurityStamp { get; set; }

        public bool LockoutEnabled { get; set; }

        public DateTimeOffset? LockoutEnd { get; set; }

        public int AccessFailedCount { get; set; }

        //public IList<UserTokenDTO> Tokens { get; set; }

        //public IList<UserClaimDTO> Claims { get; set; }

        //public IList<UserRoleDTO> UserRoles { get; set; }
    }
}
