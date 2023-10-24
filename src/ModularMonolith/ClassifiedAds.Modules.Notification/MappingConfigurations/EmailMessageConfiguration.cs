using ClassifiedAds.Modules.Notification.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassifiedAds.Modules.Notification.MappingConfigurations;

public class EmailMessageConfiguration : IEntityTypeConfiguration<EmailMessage>
{
    public void Configure(EntityTypeBuilder<EmailMessage> builder)
    {
        builder.ToTable("EmailMessages");
        builder.Property(x => x.Id).HasDefaultValueSql("newsequentialid()");
        builder.HasIndex(x => x.SentDateTime).IncludeProperties(x => new { x.ExpiredDateTime, x.AttemptCount, x.MaxAttemptCount, x.NextAttemptDateTime });
        builder.HasIndex(x => x.CreatedDateTime);
    }
}

public class ArchivedEmailMessageConfiguration : IEntityTypeConfiguration<ArchivedEmailMessage>
{
    public void Configure(EntityTypeBuilder<ArchivedEmailMessage> builder)
    {
        builder.ToTable("ArchivedEmailMessages");
        builder.HasIndex(x => x.CreatedDateTime);
    }
}