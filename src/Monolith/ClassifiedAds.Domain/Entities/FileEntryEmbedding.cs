using Microsoft.Data.SqlTypes;
using System;

namespace ClassifiedAds.Domain.Entities;

public class FileEntryEmbedding : Entity<Guid>, IAggregateRoot
{
    public string ChunkName { get; set; }

    public string ChunkLocation { get; set; }

    public string ShortText { get; set; }

    public SqlVector<float> Embedding { get; set; }

    public string TokenDetails { get; set; }

    public Guid FileEntryId { get; set; }

    public FileEntry FileEntry { get; set; }
}
