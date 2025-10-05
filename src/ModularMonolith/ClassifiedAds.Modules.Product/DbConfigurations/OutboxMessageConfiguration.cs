using ClassifiedAds.Modules.Product.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassifiedAds.Modules.Product.DbConfigurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");
        builder.Property(x => x.Id).HasDefaultValueSql("newsequentialid()");
        builder.HasIndex(x => new { x.Published, x.CreatedDateTime });
        builder.HasIndex(x => x.CreatedDateTime);
    }
}

public class ArchivedOutboxMessageConfiguration : IEntityTypeConfiguration<ArchivedOutboxMessage>
{
    public void Configure(EntityTypeBuilder<ArchivedOutboxMessage> builder)
    {
        builder.ToTable("ArchivedOutboxMessages");
        builder.HasIndex(x => x.CreatedDateTime);
    }
}
