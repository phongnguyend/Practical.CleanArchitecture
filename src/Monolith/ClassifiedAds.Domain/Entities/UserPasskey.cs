using System;

namespace ClassifiedAds.Domain.Entities;

public class UserPasskey : Entity<Guid>, IAggregateRoot
{
    public Guid UserId { get; set; }

    public byte[] CredentialId { get; set; }

    public IdentityPasskeyData Data { get; set; } = default!;
}

public class IdentityPasskeyData
{
    public byte[] PublicKey { get; set; } = default!;

    public string? Name { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public uint SignCount { get; set; }

    public string[]? Transports { get; set; }

    public bool IsUserVerified { get; set; }

    public bool IsBackupEligible { get; set; }

    public bool IsBackedUp { get; set; }

    public byte[] AttestationObject { get; set; } = default!;

    public byte[] ClientDataJson { get; set; } = default!;
}
