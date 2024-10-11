using ClassifiedAds.CrossCuttingConcerns.Pdf;
using System.Collections.Generic;

namespace ClassifiedAds.Modules.Product.Pdf;

public record ExportProductsToPdf : IPdfRequest
{
    public List<Entities.Product> Products { get; set; }
}
