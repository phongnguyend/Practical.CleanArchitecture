using System;

namespace ClassifiedAds.WebAPI.Models
{
    public class ProductInStoreModel
    {
        public Guid ProductId { get; set; }

        public Guid StoreId { get; set; }

        public int Quantity { get; set; }
    }
}
