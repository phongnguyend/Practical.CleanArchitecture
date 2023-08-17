using ClassifiedAds.CrossCuttingConcerns.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClassifiedAds.Persistence;

public class AdsDbContextMultiTenant : AdsDbContext
{
    private readonly ILogger<AdsDbContextMultiTenant> _logger;
    private readonly IConnectionStringResolver<AdsDbContextMultiTenant> _connectionStringResolver;

    public AdsDbContextMultiTenant(
        ILogger<AdsDbContextMultiTenant> logger,
        IConnectionStringResolver<AdsDbContextMultiTenant> connectionStringResolver)
        : base(new DbContextOptions<AdsDbContext>(), logger)
    {
        _logger = logger;
        _connectionStringResolver = connectionStringResolver;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer(_connectionStringResolver.ConnectionString);
    }
}
