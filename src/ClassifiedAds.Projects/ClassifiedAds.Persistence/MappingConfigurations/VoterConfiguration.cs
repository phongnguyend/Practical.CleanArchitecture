using ClassifiedAds.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassifiedAds.Persistence.MappingConfigurations
{
    public class VoterConfiguration : IEntityTypeConfiguration<Voter>
    {
        public void Configure(EntityTypeBuilder<Voter> builder)
        {
            builder.ToTable("Voters");
            builder.Property(x => x.Id).HasDefaultValueSql("newsequentialid()");
        }
    }
}
