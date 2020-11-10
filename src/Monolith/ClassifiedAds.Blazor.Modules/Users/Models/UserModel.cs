using System;
using System.ComponentModel.DataAnnotations;

namespace ClassifiedAds.Blazor.Modules.Users.Models
{
    public class UserModel
    {
        public Guid Id { get; set; }

        [Required]
        public string UserName { get; set; }

        public string NormalizedUserName { get; set; }

        [Required]
        public string Email { get; set; }

        public string NormalizedEmail { get; set; }

        public bool EmailConfirmed { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public string ConcurrencyStamp { get; set; }

        public string SecurityStamp { get; set; }

        public bool LockoutEnabled { get; set; }

        public DateTimeOffset? LockoutEnd { get; set; }

        public int AccessFailedCount { get; set; }
    }
}
