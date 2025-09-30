using System;

namespace ClassifiedAds.Domain.Entities;

public class LocalizationEntry : Entity<Guid>, IAggregateRoot
{
    public string Name { get; set; }

    public string Value { get; set; }

    public string Culture { get; set; }

    public string Description { get; set; }
}
