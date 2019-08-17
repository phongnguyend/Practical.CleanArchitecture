using ClassifiedAds.DomainServices;
using ClassifiedAds.DomainServices.Repositories;
using ClassifiedAds.Persistence;
using ClassifiedAds.Persistence.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PersistanceServiceCollectionExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AdsDbContext>(options => options.UseSqlServer(connectionString))
                    .AddScoped<IUnitOfWork, UnitOfWork>()
                    .AddScoped(typeof(IRepository<>), typeof(Repository<>));
            return services;
        }

        public static void MigrateDb(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<AdsDbContext>().Database.Migrate();
            }
        }
    }
}
