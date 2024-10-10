using ClassifiedAds.CrossCuttingConcerns.Html;
using ClassifiedAds.Domain.Entities;
using System.Collections.Generic;

namespace ClassifiedAds.Application.Products.DTOs;

public record ExportProductsToHtml : IHtmlRequest
{
    public List<Product> Products { get; set; }
}
