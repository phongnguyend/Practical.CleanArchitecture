using ClassifiedAds.Persistence.SqlServer;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ClassifiedAds.Modules.AuditLog.Persistence;

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
