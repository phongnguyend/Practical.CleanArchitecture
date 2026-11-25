using System;

namespace ClassifiedAds.Domain.Entities;

public sealed class MobilePushDevice
{
    public Guid Id { get; set; }

    public string UserId { get; set; } = "";

    public string App { get; set; } = "";

    public string Platform { get; set; } = "";

    public string Token { get; set; } = "";

    public string TokenHash { get; set; } = "";

    public DateTime UpdatedAt { get; set; }
}

public sealed class MobilePushMessage
{
    public Guid Id { get; set; }

    public string UserId { get; set; } = "";

    public string Title { get; set; } = "";

    public string Body { get; set; } = "";

    public string Section { get; set; } = "notifications";

    public DateTime CreatedAt { get; set; }

    public DateTime? ReadAt { get; set; }
}

public sealed class MobilePushDelivery
{
    public Guid Id { get; set; }

    public Guid MessageId { get; set; }

    public Guid DeviceId { get; set; }

    public string UserId { get; set; } = "";

    public int Attempts { get; set; }

    public DateTime NextAttemptAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public string? LastError { get; set; }
}

