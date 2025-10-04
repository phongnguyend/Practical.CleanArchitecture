using ClassifiedAds.Persistence.SqlServer;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ClassifiedAds.Modules.Product.Persistence;

public class ProductDbContext : DbContextUnitOfWork<ProductDbContext>
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
