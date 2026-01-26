using ClassifiedAds.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace ClassifiedAds.Persistence;

public class AdsReadOnlyDbContext : DbContext
{
    private readonly ILogger<AdsReadOnlyDbContext> _logger;

    public AdsReadOnlyDbContext(DbContextOptions<AdsReadOnlyDbContext> options,
        ILogger<AdsReadOnlyDbContext> logger)
        : base(options)
    {
        _logger = logger;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new SelectWithoutWhereCommandInterceptor(_logger));
        optionsBuilder.AddInterceptors(new SelectWhereInCommandInterceptor(_logger));
    }
}
