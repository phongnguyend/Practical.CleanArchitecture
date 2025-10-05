using ClassifiedAds.Modules.Storage.Entities;
using ClassifiedAds.Persistence.SqlServer;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Modules.Storage.Persistence;

public class StorageDbContext : DbContextUnitOfWork<StorageDbContext>
{
    public StorageDbContext(DbContextOptions<StorageDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override int SaveChanges()
    {
        SetOutboxActivityId();
        HandleFileEntriesDeleted();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetOutboxActivityId();
        HandleFileEntriesDeleted();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void SetOutboxActivityId()
    {
        var entities = ChangeTracker.Entries<OutboxMessage>();
        foreach (var entity in entities.Where(e => e.State == EntityState.Added))
        {
            var outbox = entity.Entity;

            if (string.IsNullOrWhiteSpace(outbox.ActivityId))
            {
                outbox.ActivityId = System.Diagnostics.Activity.Current?.Id;
            }
        }
    }

    private void HandleFileEntriesDeleted()
    {
        var entities = ChangeTracker.Entries<FileEntry>();
        foreach (var entity in entities.Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
        {
            var fileEntry = entity.Entity;

            if (fileEntry.Deleted)
            {
                Set<DeletedFileEntry>().Add(new DeletedFileEntry
                {
                    FileEntryId = fileEntry.Id,
                    CreatedDateTime = fileEntry.DeletedDate ?? System.DateTimeOffset.Now
                });
            }
        }
    }
}
