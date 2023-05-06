using ClassifiedAds.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ClassifiedAds.Services.AuditLog.Repositories;

public class AuditLogDbContext : DbContextUnitOfWork<AuditLogDbContext>
{
    public AuditLogDbContext(DbContextOptions<AuditLogDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
