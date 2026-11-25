using ClassifiedAds.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Notification.MobilePush;

public sealed class MobilePushStore
{
    private readonly DbContext context;

    public MobilePushStore(DbContext context)
    {
        this.context = context;
    }

    public DbSet<MobilePushDevice> Devices => context.Set<MobilePushDevice>();

    public DbSet<MobilePushMessage> Messages => context.Set<MobilePushMessage>();

    public DbSet<MobilePushDelivery> Deliveries => context.Set<MobilePushDelivery>();

    public DatabaseFacade Database => context.Database;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken) => context.SaveChangesAsync(cancellationToken);
}
