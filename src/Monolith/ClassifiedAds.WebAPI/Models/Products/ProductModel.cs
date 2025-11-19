using System;
using System.Collections.Generic;

namespace ClassifiedAds.WebAPI.Models.Products;

public class ProductModel
{
    public Guid Id { get; set; }

    public string Code { get; set; }

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