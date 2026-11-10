using System;

namespace ClassifiedAds.Modules.Identity.Models;

// An explicit allowlist prevents identity secrets in old logs from reaching clients.
public class UserAuditSnapshot
{
    public Guid Id { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public bool EmailConfirmed { get; set; }

    public string PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public bool LockoutEnabled { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }

    public int AccessFailedCount { get; set; }
}
