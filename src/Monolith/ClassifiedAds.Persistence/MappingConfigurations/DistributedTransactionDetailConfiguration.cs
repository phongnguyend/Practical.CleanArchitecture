using ClassifiedAds.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassifiedAds.Persistence.MappingConfigurations
{
    public class DistributedTransactionDetailConfiguration : IEntityTypeConfiguration<DistributedTransactionDetail>
    {
        public void Configure(EntityTypeBuilder<DistributedTransactionDetail> builder)
        {
            builder.ToTable("DistributedTransactionDetails");
            builder.Property(x => x.Id).HasDefaultValueSql("newsequentialid()");
        }
    }
}
