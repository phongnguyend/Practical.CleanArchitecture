using ClassifiedAds.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassifiedAds.Persistence.MappingConfigurations
{
    public class AuditLogEntryConfiguration : IEntityTypeConfiguration<AuditLogEntry>
    {
        public void Configure(EntityTypeBuilder<AuditLogEntry> builder)
        {
            builder.ToTable("AuditLogEntries");
            builder.Property(x => x.Id).HasDefaultValueSql("newsequentialid()");
        }
    }
}
