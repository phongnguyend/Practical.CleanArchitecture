using ClassifiedAds.Services.AuditLog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassifiedAds.Services.AuditLog.MappingConfigurations;

public class AuditLogEntryConfiguration : IEntityTypeConfiguration<AuditLogEntry>
{
    public void Configure(EntityTypeBuilder<AuditLogEntry> builder)
    {
        builder.ToTable("AuditLogEntries");
        builder.Property(x => x.Id).HasDefaultValueSql("newsequentialid()");
    }
}
