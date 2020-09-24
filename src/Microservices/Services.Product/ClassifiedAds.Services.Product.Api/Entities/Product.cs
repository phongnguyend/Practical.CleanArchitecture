using ClassifiedAds.Domain.Entities;
using System;

namespace ClassifiedAds.Services.Product.Entities
{
    public class Product : AggregateRoot<Guid>
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
