using System;
using System.Collections.Generic;

namespace ClassifiedAds.Blazor.Modules.Products.Models
{
    public class ProductAuditLogModel
    {
        public string UserName { get; set; }

        public string Action { get; set; }

        public DateTimeOffset CreatedDateTime { get; set; }

        public Dictionary<string, bool> HighLight { get; set; }

        public ProductModel Data { get; set; }
    }
}
