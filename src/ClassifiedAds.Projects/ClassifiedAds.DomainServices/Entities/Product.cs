using System;
using System.Collections.Generic;
using System.Text;

namespace ClassifiedAds.DomainServices.Entities
{
    public class Product : Entity<Guid>
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
