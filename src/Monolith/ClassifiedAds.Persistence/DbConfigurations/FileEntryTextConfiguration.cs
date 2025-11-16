using ClassifiedAds.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassifiedAds.Persistence.DbConfigurations;

public class FileEntryTextConfiguration : IEntityTypeConfiguration<FileEntryText>
{
    public void Configure(EntityTypeBuilder<FileEntryText> builder)
    {
        builder.ToTable("FileEntryTexts");
        builder.Property(x => x.Id).HasDefaultValueSql("newsequentialid()");
    }
}