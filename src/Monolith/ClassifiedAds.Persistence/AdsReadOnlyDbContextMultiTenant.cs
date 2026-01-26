using ClassifiedAds.CrossCuttingConcerns.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClassifiedAds.Persistence;

public class AdsReadOnlyDbContextMultiTenant : AdsReadOnlyDbContext
{
    private readonly ILogger<AdsReadOnlyDbContextMultiTenant> _logger;
    private readonly IConnectionStringResolver<AdsReadOnlyDbContextMultiTenant> _connectionStringResolver;

    public AdsReadOnlyDbContextMultiTenant(
        ILogger<AdsReadOnlyDbContextMultiTenant> logger,
        IConnectionStringResolver<AdsReadOnlyDbContextMultiTenant> connectionStringResolver)
        : base(new DbContextOptions<AdsReadOnlyDbContext>(), logger)
    {
        _logger = logger;
        _connectionStringResolver = connectionStringResolver;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer(_connectionStringResolver.ConnectionString);
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }
}
