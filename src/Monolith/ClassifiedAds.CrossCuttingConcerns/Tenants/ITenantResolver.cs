namespace ClassifiedAds.CrossCuttingConcerns.Tenants
{
    public interface ITenantResolver
    {
        string Id { get; }
        string Name { get; }
    }
}
