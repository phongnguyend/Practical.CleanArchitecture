using System;

namespace ClassifiedAds.Domain.Entities;

public class FileEntryEmbedding : Entity<Guid>, IAggregateRoot
{
    public string ChunkName { get; set; }

    public string Embedding { get; set; }

    public string TokenDetails { get; set; }

    public Guid FileEntryId { get; set; }

    public FileEntry FileEntry { get; set; }
}
