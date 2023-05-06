using ClassifiedAds.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ClassifiedAds.Services.Storage.Repositories;

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
