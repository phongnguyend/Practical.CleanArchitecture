using ClassifiedAds.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace ClassifiedAds.Persistence.DbConfigurations;

public class ConfigurationEntryConfiguration : IEntityTypeConfiguration<ConfigurationEntry>
{
    public void Configure(EntityTypeBuilder<ConfigurationEntry> builder)
    {
        builder.ToTable("ConfigurationEntries");
        builder.Property(x => x.Id).HasDefaultValueSql("newsequentialid()");

        // Seed
        builder.HasData(new List<ConfigurationEntry>
        {
            new ConfigurationEntry
            {
                Id = Guid.Parse("8A051AA5-BCD1-EA11-B098-AC728981BD15"),
                Key = "SecurityHeaders:Test-Read-From-SqlServer",
                Value = "this-is-read-from-sqlserver",
            },
        });
    }
}