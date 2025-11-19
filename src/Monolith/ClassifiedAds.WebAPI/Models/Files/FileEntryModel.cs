using System;
using System.Collections.Generic;

namespace ClassifiedAds.WebAPI.Models.Files;

public class FileEntryModel
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public long Size { get; set; }

    public DateTimeOffset UploadedTime { get; set; }

    public string FileName { get; set; }

    public string FileLocation { get; set; }

    public bool Encrypted { get; set; }

    public FileEntryTextModel FileEntryText { get; set; }

    public List<FileEntryEmbeddingModel> FileEntryEmbeddings { get; set; }
}

public class FileEntryTextModel
{
    public string TextLocation { get; set; }
}

public class FileEntryEmbeddingModel
{
    public string ChunkName { get; set; }

    public string ChunkLocation { get; set; }

    public string ShortText { get; set; }

    public string Embedding { get; set; }

    public string TokenDetails { get; set; }

    public DateTimeOffset CreatedDateTime { get; set; }

    public DateTimeOffset? UpdatedDateTime { get; set; }
}

public class FileEntryVectorSearchResultModel
{
    public Guid FileEntryId { get; set; }

    public string FileEntryName { get; set; }

    public string FileName { get; set; }

    public string FileExtension { get; set; }

    public string ChunkName { get; set; }

    public string ChunkData { get; set; }

    public double SimilarityScore { get; set; }
}
