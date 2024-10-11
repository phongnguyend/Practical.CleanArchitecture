using ClassifiedAds.CrossCuttingConcerns.Csv;
using System.Collections.Generic;

namespace ClassifiedAds.Modules.Product.Csv;

public record ImportProductsFromCsv : ICsvResponse
{
    public List<Entities.Product> Products { get; set; }
}
