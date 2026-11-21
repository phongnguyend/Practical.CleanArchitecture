using System.ComponentModel.DataAnnotations;

namespace ClassifiedAds.Mobile.Core.Models;

public sealed class User : Record
{
    public string UserName { get; set; } = "";

    public string Email { get; set; } = "";

    public bool EmailConfirmed { get; set; }

    public string? PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public bool LockoutEnabled { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }

    public int AccessFailedCount { get; set; }

    public override string Title => UserName;

    public override object Payload() => new
    {
        id = string.IsNullOrEmpty(Id) ? null : Id,
        UserName,
        Email,
        EmailConfirmed,
        PhoneNumber,
        PhoneNumberConfirmed,
        TwoFactorEnabled,
        LockoutEnabled,
        LockoutEnd,
        AccessFailedCount
    };
    public override string? Validate() => Required(UserName, "Username", 3) ?? Required(Email, "Email") ?? (!new EmailAddressAttribute().IsValid(Email) ? "Enter a valid email." : null) ?? (AccessFailedCount < 0 ? "Failed attempts must be a nonnegative integer." : null);
}
