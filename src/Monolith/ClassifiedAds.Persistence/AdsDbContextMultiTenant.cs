using ClassifiedAds.CrossCuttingConcerns.Tenants;
using Microsoft.EntityFrameworkCore;

namespace ClassifiedAds.Persistence
{
    public class AdsDbContextMultiTenant : AdsDbContext
    {
        private readonly IConnectionStringResolver<AdsDbContextMultiTenant> _connectionStringResolver;
        private readonly ITenantResolver _tenantResolver;

        public AdsDbContextMultiTenant(
            IConnectionStringResolver<AdsDbContextMultiTenant> connectionStringResolver,
            ITenantResolver tenantResolver)
            : base(new DbContextOptions<AdsDbContext>())
        {
            _connectionStringResolver = connectionStringResolver;
            _tenantResolver = tenantResolver;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionStringResolver.ConnectionString);
        }
    }
}
