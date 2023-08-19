using ClassifiedAds.Services.AuditLog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassifiedAds.Services.AuditLog.MappingConfigurations;

public class IdempotentRequestConfiguration : IEntityTypeConfiguration<IdempotentRequest>
{
    public void Configure(EntityTypeBuilder<IdempotentRequest> builder)
    {
        builder.ToTable("IdempotentRequests");
        builder.Property(x => x.Id).HasDefaultValueSql("newsequentialid()");
        builder.Property(x => x.RequestType).IsRequired();
        builder.Property(x => x.RequestId).IsRequired();
        builder.HasIndex(x => new { x.RequestType, x.RequestId }).IsUnique();
    }
}
