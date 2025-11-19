using ClassifiedAds.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassifiedAds.Persistence.DbConfigurations;

public class EmbeddingCacheEntryConfiguration : IEntityTypeConfiguration<EmbeddingCacheEntry>
{
    public void Configure(EntityTypeBuilder<EmbeddingCacheEntry> builder)
    {
        builder.ToTable("EmbeddingCacheEntries");
        builder.Property(x => x.Id).HasDefaultValueSql("newsequentialid()");
        builder.Property(x => x.Text).IsRequired();
        builder.HasIndex(x => x.Text).IsUnique();
    }
}