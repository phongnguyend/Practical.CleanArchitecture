using ClassifiedAds.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassifiedAds.Persistence.DbConfigurations;

public class FileEntryEmbeddingConfiguration : IEntityTypeConfiguration<FileEntryEmbedding>
{
    public void Configure(EntityTypeBuilder<FileEntryEmbedding> builder)
    {
        builder.ToTable("FileEntryEmbeddings");
        builder.Property(x => x.Id).HasDefaultValueSql("newsequentialid()");
        builder.Property(b => b.Embedding).HasColumnType("vector(1536)");
    }
}