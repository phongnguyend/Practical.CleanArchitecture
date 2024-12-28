namespace ClassifiedAds.CrossCuttingConcerns.Tenants;

public interface IConnectionStringResolver<TDbContext>
{
    string ConnectionString { get; }

    string MigrationsAssembly { get; }
}
