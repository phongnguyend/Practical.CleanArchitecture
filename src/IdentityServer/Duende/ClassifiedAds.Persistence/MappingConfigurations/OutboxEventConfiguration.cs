using ClassifiedAds.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassifiedAds.Persistence.MappingConfigurations
{
    public class OutboxEventConfiguration : IEntityTypeConfiguration<OutboxEvent>
    {
        public void Configure(EntityTypeBuilder<OutboxEvent> builder)
        {
            builder.ToTable("OutboxEvents");
            builder.Property(x => x.Id).HasDefaultValueSql("newsequentialid()");
        }
    }
}
