using ClassifiedAds.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace ClassifiedAds.Persistence.DbConfigurations;

public class LocalizationEntryConfiguration : IEntityTypeConfiguration<LocalizationEntry>
{
    public void Configure(EntityTypeBuilder<LocalizationEntry> builder)
    {
        builder.ToTable("LocalizationEntries");
        builder.Property(x => x.Id).HasDefaultValueSql("newsequentialid()");

        // Seed
        builder.HasData(new List<LocalizationEntry>
        {
            new LocalizationEntry
            {
                Id = Guid.Parse("29A4AACB-4DDF-4F85-ACED-C5283A8BDD7F"),
                Name = "Test",
                Value = "Test",
                Culture = "en-US",
            },
            new LocalizationEntry
            {
                Id = Guid.Parse("5A262D8A-B0D9-45D3-8C0E-18B2C882B9FE"),
                Name = "Test",
                Value = "Kiem Tra",
                Culture = "vi-VN",
            },
        });
    }
}