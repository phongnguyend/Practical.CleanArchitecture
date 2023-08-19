using ClassifiedAds.Domain.Entities;
using System;

namespace ClassifiedAds.Modules.AuditLog.Entities;

public class IdempotentRequest : Entity<Guid>, IAggregateRoot
{
    public string RequestType { get; set; }

    public string RequestId { get; set; }
}
