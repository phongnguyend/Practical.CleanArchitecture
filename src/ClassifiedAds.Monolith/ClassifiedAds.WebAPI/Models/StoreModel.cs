using System.Collections.Generic;

namespace ClassifiedAds.WebAPI.Models
{
    public class StoreModel
    {
        public string Name { get; set; }

        public AddressModel Location { get; set; }

        public int OpenedTime { get; set; }

        public int ClosedTime { get; set; }

        public IList<ProductInStoreModel> Products { get; set; }
    }
}
