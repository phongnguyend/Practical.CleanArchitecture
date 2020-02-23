using ClassifiedAds.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassifiedAds.Persistence.MappingConfigurations
{
    public class ProductInStoreConfiguration : IEntityTypeConfiguration<ProductInStore>
    {
        public void Configure(EntityTypeBuilder<ProductInStore> builder)
        {
            builder.ToTable("ProductInStores");
            builder.Property(x => x.Id).HasDefaultValueSql("newsequentialid()");
        }
    }
}
