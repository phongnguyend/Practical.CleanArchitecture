using System;

namespace ClassifiedAds.Domain.Infrastructure.Messaging;

public class MetaData
{
    public string MessageId { get; set; }

    public string MessageVersion { get; set; }

    public string ActivityId { get; set; }

    public DateTimeOffset? CreationDateTime { get; set; }

    public DateTimeOffset? EnqueuedDateTime { get; set; }
}
