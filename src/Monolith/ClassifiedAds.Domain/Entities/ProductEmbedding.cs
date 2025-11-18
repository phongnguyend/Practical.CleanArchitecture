using Microsoft.Data.SqlTypes;
using System;

namespace ClassifiedAds.Domain.Entities;

public class ProductEmbedding : Entity<Guid>, IAggregateRoot
{
    public string Text { get; set; }

    public SqlVector<float> Embedding { get; set; }

    public string TokenDetails { get; set; }

    public Guid ProductId { get; set; }

    public Product Product { get; set; }
}