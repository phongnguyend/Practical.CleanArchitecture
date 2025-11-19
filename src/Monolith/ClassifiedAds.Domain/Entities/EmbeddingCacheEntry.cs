using System;

namespace ClassifiedAds.Domain.Entities;

public class EmbeddingCacheEntry : Entity<Guid>, IAggregateRoot
{
    public string Text { get; set; }

    public string Provider { get; set; }

    public string Model { get; set; }

    public string Embedding { get; set; }

    public long? InputTokenCount { get; set; }

    public long? OutputTokenCount { get; set; }

    public long? TotalTokenCount { get; set; }
}