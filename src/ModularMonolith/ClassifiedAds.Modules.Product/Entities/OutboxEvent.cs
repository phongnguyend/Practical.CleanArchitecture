using ClassifiedAds.Domain.Entities;
using System;

namespace ClassifiedAds.Modules.Product.Entities;

public class OutboxEvent : OutboxEventBase, IAggregateRoot
{
}

public class ArchivedOutboxEvent : OutboxEventBase, IAggregateRoot
{
}

public abstract class OutboxEventBase : Entity<Guid>
{
    public string EventType { get; set; }

    public Guid TriggeredById { get; set; }

    public string ObjectId { get; set; }

    public string Message { get; set; }

    public bool Published { get; set; }
}
