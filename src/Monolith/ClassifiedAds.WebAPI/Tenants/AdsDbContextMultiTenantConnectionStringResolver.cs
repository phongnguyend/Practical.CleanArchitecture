using ClassifiedAds.CrossCuttingConcerns.Tenants;
using ClassifiedAds.Persistence;
using ClassifiedAds.WebAPI.ConfigurationOptions;
using Microsoft.Extensions.Options;

namespace ClassifiedAds.WebAPI.Tenants
{
    public class AdsDbContextMultiTenantConnectionStringResolver : IConnectionStringResolver<AdsDbContextMultiTenant>
    {
        private readonly AppSettings _appSettings;

        public AdsDbContextMultiTenantConnectionStringResolver(IOptionsSnapshot<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public string ConnectionString => _appSettings.ConnectionStrings.ClassifiedAds;

        public string MigrationsAssembly => string.Empty;
    }
}
