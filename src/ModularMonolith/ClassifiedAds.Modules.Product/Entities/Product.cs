using ClassifiedAds.Domain.Entities;
using System;

namespace ClassifiedAds.Modules.Product.Entities;

public class Product : Entity<Guid>, IAggregateRoot
{
    public string Code { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }
}
