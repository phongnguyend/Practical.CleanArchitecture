using ClassifiedAds.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassifiedAds.Persistence.DbConfigurations;

public sealed class MobilePushNotificationConfiguration : IEntityTypeConfiguration<MobilePushDevice>, IEntityTypeConfiguration<MobilePushMessage>, IEntityTypeConfiguration<MobilePushDelivery>
{
    public void Configure(EntityTypeBuilder<MobilePushDevice> devices)
    {
        devices.ToTable("PushDevices");
        devices.HasKey(x => x.Id);
        devices.Property(x => x.UserId).HasMaxLength(200);
        devices.Property(x => x.App).HasMaxLength(30);
        devices.Property(x => x.Platform).HasMaxLength(10);
        devices.Property(x => x.Token).HasMaxLength(2048);
        devices.Property(x => x.TokenHash).HasMaxLength(64);
        devices.HasIndex(x => x.TokenHash).IsUnique();
        devices.HasIndex(x => x.UserId);
    }

    public void Configure(EntityTypeBuilder<MobilePushMessage> messages)
    {
        messages.ToTable("PushMessages");
        messages.HasKey(x => x.Id);
        messages.Property(x => x.UserId).HasMaxLength(200);
        messages.Property(x => x.Title).HasMaxLength(100);
        messages.Property(x => x.Body).HasMaxLength(500);
        messages.Property(x => x.Section).HasMaxLength(40);
        messages.HasIndex(x => new { x.UserId, x.CreatedAt });
    }

    public void Configure(EntityTypeBuilder<MobilePushDelivery> deliveries)
    {
        deliveries.ToTable("PushDeliveries");
        deliveries.HasKey(x => x.Id);
        deliveries.Property(x => x.UserId).HasMaxLength(200);
        deliveries.Property(x => x.LastError).HasMaxLength(100);
        deliveries.HasIndex(x => new { x.CompletedAt, x.NextAttemptAt });
        deliveries.HasIndex(x => new { x.MessageId, x.DeviceId }).IsUnique();
    }
}
