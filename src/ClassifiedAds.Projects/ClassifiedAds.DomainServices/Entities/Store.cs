using ClassifiedAds.DomainServices.ValueObjects;
using System;
using System.Collections.Generic;

namespace ClassifiedAds.DomainServices.Entities
{
    public class Store : Entity<Guid>
    {
        public string Name { get; set; }

        public Address Location { get; set; }

        public int OpenedTime { get; set; }

        public int ClosedTime { get; set; }

        public IList<ProductInStore> Products { get; set; }
    }
}
