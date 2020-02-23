using ClassifiedAds.Domain.ValueObjects;
using System;
using System.Collections.Generic;

namespace ClassifiedAds.Domain.Entities
{
    public class Store : AggregateRoot<Guid>
    {
        public string Name { get; set; }

        public Address Location { get; set; }

        public int OpenedTime { get; set; }

        public int ClosedTime { get; set; }

        public IList<ProductInStore> Products { get; set; }
    }
}
