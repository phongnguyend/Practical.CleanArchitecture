using ClassifiedAds.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassifiedAds.Persistence.DbConfigurations;

public class UserPasskeyConfiguration : IEntityTypeConfiguration<UserPasskey>
{
    public void Configure(EntityTypeBuilder<UserPasskey> builder)
    {
        builder.ToTable("UserPasskeys");
        builder.Property(x => x.Id).HasDefaultValueSql("newsequentialid()");
        builder.OwnsOne(p => p.Data).ToJson();
    }
}
