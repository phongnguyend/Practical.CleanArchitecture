using ClassifiedAds.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassifiedAds.Persistence.MappingConfigurations
{
    public class DistributedTransactionPropertyConfiguration : IEntityTypeConfiguration<DistributedTransactionProperty>
    {
        public void Configure(EntityTypeBuilder<DistributedTransactionProperty> builder)
        {
            builder.ToTable("DistributedTransactionProperties");
            builder.Property(x => x.Id).HasDefaultValueSql("newsequentialid()");
        }
    }
}
