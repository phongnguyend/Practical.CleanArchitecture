using ClassifiedAds.CrossCuttingConcerns.Tenants;

namespace ClassifiedAds.WebAPI.Tenants
{
    public class TenantResolver : ITenantResolver
    {
        public string Id => string.Empty;

        public string Name => string.Empty;
    }
}
