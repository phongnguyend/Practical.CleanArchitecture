using ClassifiedAds.Domain.Entities;
using System.Collections.Generic;

namespace ClassifiedAds.Application.Products.DTOs;

public record ImportProductsFromCsv
{
    public List<Product> Products { get; set; }
}
