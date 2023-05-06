using ClassifiedAds.Services.Storage.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassifiedAds.Services.Storage.MappingConfigurations;

public class FileEntryConfiguration : IEntityTypeConfiguration<FileEntry>
{
    public void Configure(EntityTypeBuilder<FileEntry> builder)
    {
        builder.ToTable("FileEntries");
        builder.Property(x => x.Id).HasDefaultValueSql("newsequentialid()");
    }
}