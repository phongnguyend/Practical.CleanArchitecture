using ClassifiedAds.Persistence.SqlServer;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

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
}
