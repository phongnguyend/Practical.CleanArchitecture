using ClassifiedAds.Domain.Entities;
using System;

namespace ClassifiedAds.Services.AuditLog.Entities;

public class IdempotentRequest : Entity<Guid>, IAggregateRoot
{
    public string RequestType { get; set; }

    public string RequestId { get; set; }
}
