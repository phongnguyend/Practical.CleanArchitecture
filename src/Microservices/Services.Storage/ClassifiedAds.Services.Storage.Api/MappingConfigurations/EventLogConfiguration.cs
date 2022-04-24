using ClassifiedAds.Services.Storage.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassifiedAds.Services.Storage.MappingConfigurations
{
    public class EventLogConfiguration : IEntityTypeConfiguration<EventLog>
    {
        public void Configure(EntityTypeBuilder<EventLog> builder)
        {
            builder.ToTable("EventLogs");
            builder.Property(x => x.Id).UseIdentityColumn();
        }
    }
}
