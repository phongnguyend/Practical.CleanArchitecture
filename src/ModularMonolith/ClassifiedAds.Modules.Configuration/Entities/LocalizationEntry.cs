using ClassifiedAds.Domain.Entities;
using System;

namespace ClassifiedAds.Modules.Configuration.Entities;

public class LocalizationEntry : Entity<Guid>, IAggregateRoot
{
    public string Name { get; set; }

    public string Value { get; set; }

    public string Culture { get; set; }

    public string Description { get; set; }
}
