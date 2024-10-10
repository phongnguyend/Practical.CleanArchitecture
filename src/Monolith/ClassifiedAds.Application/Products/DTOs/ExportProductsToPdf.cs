using ClassifiedAds.CrossCuttingConcerns.Pdf;
using ClassifiedAds.Domain.Entities;
using System.Collections.Generic;

namespace ClassifiedAds.Application.Products.DTOs;

public record ExportProductsToPdf : IPdfRequest
{
    public List<Product> Products { get; set; }
}
