using System;

namespace ClassifiedAds.Domain.Entities;

public class Lock
{
    public string EntityId { get; set; }

    public string EntityName { get; set; }

    public string OwnerId { get; set; }

    public DateTimeOffset? AcquiredDateTime { get; set; }

    public DateTimeOffset? ExpiredDateTime { get; set; }
}
