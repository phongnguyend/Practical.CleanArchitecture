using System;

namespace ClassifiedAds.DomainServices.Entities
{
    public class ProductInStore : Entity<Guid>
    {
        public Guid ProductId { get; set; }

        public string Code { get; set; }

        public Guid StoreId { get; set; }

        public int Quantity { get; set; }
    }
}
