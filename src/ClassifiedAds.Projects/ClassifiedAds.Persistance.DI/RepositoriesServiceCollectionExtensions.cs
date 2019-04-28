using ClassifiedAds.DomainServices;
using ClassifiedAds.Persistance;
using ClassifiedAds.Persistance.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PersistanceServiceCollectionExtensions
    {
        public static IServiceCollection AddPersistance(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AdsDbContext>(options => options.UseSqlServer(connectionString))
                    .AddScoped(typeof(IRepository<>), typeof(Repository<>));
            return services;
        }
    }
}
