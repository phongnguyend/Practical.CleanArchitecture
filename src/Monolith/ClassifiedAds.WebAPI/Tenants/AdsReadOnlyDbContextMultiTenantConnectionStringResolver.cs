using ClassifiedAds.CrossCuttingConcerns.Tenants;
using ClassifiedAds.Persistence;
using ClassifiedAds.WebAPI.ConfigurationOptions;
using Microsoft.Extensions.Options;

namespace ClassifiedAds.WebAPI.Tenants;

public class AdsReadOnlyDbContextMultiTenantConnectionStringResolver : IConnectionStringResolver<AdsReadOnlyDbContextMultiTenant>
{
    private readonly AppSettings _appSettings;
    private readonly ITenantResolver _tenantResolver;

    public AdsReadOnlyDbContextMultiTenantConnectionStringResolver(IOptionsSnapshot<AppSettings> appSettings,
        ITenantResolver tenantResolver)
    {
        _appSettings = appSettings.Value;
        _tenantResolver = tenantResolver;
    }

    public string ConnectionString
    {
        get
        {
            var tenant = _tenantResolver.Tenant;
            if (tenant is null)
            {
                return _appSettings.ConnectionStrings.ClassifiedAds;
            }

            // TODO: query configuration store to get ConnectionString
            return _appSettings.ConnectionStrings.ClassifiedAds;
        }
    }
}
