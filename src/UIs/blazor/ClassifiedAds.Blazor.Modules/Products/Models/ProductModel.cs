using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClassifiedAds.Blazor.Modules.Products.Models;

public class ProductModel
{
    public Guid Id { get; set; }

    [Required]
    [MinLength(3)]
    public string Code { get; set; }

    [Required]
    [MaxLength(10)]
    public string Name { get; set; }

    public string Description { get; set; }

    public double SimilarityScore { get; set; }

    public ProductEmbeddingModel ProductEmbedding { get; set; }

    public List<SimilarProductModel> SimilarProducts { get; set; }
}

public class ProductEmbeddingModel
{
    public string Text { get; set; }

    public string Embedding { get; set; }

    public string TokenDetails { get; set; }

    public DateTimeOffset CreatedDateTime { get; set; }

    public DateTimeOffset? UpdatedDateTime { get; set; }
}

public class SimilarProductModel : ProductModel
{
}