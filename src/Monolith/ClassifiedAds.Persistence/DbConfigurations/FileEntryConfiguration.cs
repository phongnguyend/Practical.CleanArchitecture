using ClassifiedAds.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassifiedAds.Persistence.DbConfigurations;

public class FileEntryConfiguration : IEntityTypeConfiguration<FileEntry>
{
    public void Configure(EntityTypeBuilder<FileEntry> builder)
    {
        builder.ToTable("FileEntries");
        builder.Property(x => x.Id).HasDefaultValueSql("newsequentialid()");
    }
}

public class DeletedFileEntryConfiguration : IEntityTypeConfiguration<DeletedFileEntry>
{
    public void Configure(EntityTypeBuilder<DeletedFileEntry> builder)
    {
        builder.ToTable("DeletedFileEntries");
        builder.Property(x => x.Id).HasDefaultValueSql("newsequentialid()");
    }
}