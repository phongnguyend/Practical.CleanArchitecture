using ClassifiedAds.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassifiedAds.Persistence.MappingConfigurations
{
    public class DistributedTransactionConfiguration : IEntityTypeConfiguration<DistributedTransaction>
    {
        public void Configure(EntityTypeBuilder<DistributedTransaction> builder)
        {
            builder.ToTable("DistributedTransactions");
            builder.Property(x => x.Id).HasDefaultValueSql("newsequentialid()");
        }
    }
}
