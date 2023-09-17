using ClassifiedAds.Domain.Notification;
using System;

namespace ClassifiedAds.Domain.Entities;

public class SmsMessage : SmsMessageBase, IAggregateRoot, ISmsMessage
{
}

public class ArchivedSmsMessage : SmsMessageBase, IAggregateRoot
{
}

public abstract class SmsMessageBase : Entity<Guid>
{
    public string Message { get; set; }

    public string PhoneNumber { get; set; }

    public int AttemptCount { get; set; }

    public int MaxAttemptCount { get; set; }

    public DateTimeOffset? NextAttemptDateTime { get; set; }

    public DateTimeOffset? ExpiredDateTime { get; set; }

    public string Log { get; set; }

    public DateTimeOffset? SentDateTime { get; set; }

    public Guid? CopyFromId { get; set; }
}
