using ClassifiedAds.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassifiedAds.Persistence.DbConfigurations;

public class ProductEmbeddingConfiguration : IEntityTypeConfiguration<ProductEmbedding>
{
    public void Configure(EntityTypeBuilder<ProductEmbedding> builder)
    {
        builder.ToTable("ProductEmbeddings");
        builder.Property(x => x.Id).HasDefaultValueSql("newsequentialid()");
        builder.Property(b => b.Embedding).HasColumnType("vector(1536)");
    }
}
