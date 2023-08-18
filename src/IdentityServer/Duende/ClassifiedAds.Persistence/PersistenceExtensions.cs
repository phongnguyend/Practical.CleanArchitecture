using ClassifiedAds.CrossCuttingConcerns.CircuitBreakers;
using ClassifiedAds.CrossCuttingConcerns.Locks;
using ClassifiedAds.CrossCuttingConcerns.Tenants;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Persistence;
using ClassifiedAds.Persistence.CircuitBreakers;
using ClassifiedAds.Persistence.Locks;
using ClassifiedAds.Persistence.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PersistenceExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString, string migrationsAssembly = "")
        {
            services.AddDbContext<AdsDbContext>(options => options.UseSqlServer(connectionString, sql =>
                    {
                        if (!string.IsNullOrEmpty(migrationsAssembly))
                        {
                            sql.MigrationsAssembly(migrationsAssembly);
                        }
                    }))
                    .AddDbContextFactory<AdsDbContext>((Action<DbContextOptionsBuilder>)null, ServiceLifetime.Scoped)
                    .AddRepositories();

            services.AddScoped(typeof(IDistributedLock), _ => new SqlDistributedLock(connectionString));

            return services;
        }

        public static IServiceCollection AddMultiTenantPersistence(this IServiceCollection services, Type connectionStringResolverType, Type tenantResolverType)
        {
            services.AddScoped(typeof(IConnectionStringResolver<AdsDbContextMultiTenant>), connectionStringResolverType);
            services.AddScoped(typeof(ITenantResolver), tenantResolverType);

            services.AddDbContext<AdsDbContextMultiTenant>(options => { })
                    .AddScoped(typeof(AdsDbContext), services =>
                    {
                        return services.GetRequiredService<AdsDbContextMultiTenant>();
                    })
                    .AddRepositories();

            services.AddScoped(typeof(IDistributedLock), services =>
            {
                return new SqlDistributedLock(services.GetRequiredService<IConnectionStringResolver<AdsDbContextMultiTenant>>().ConnectionString);
            });

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>))
                    .AddScoped(typeof(IAuditLogEntryRepository), typeof(AuditLogEntryRepository))
                    .AddScoped(typeof(IUserRepository), typeof(UserRepository))
                    .AddScoped(typeof(IRoleRepository), typeof(RoleRepository));

            services.AddScoped(typeof(IUnitOfWork), services =>
            {
                return services.GetRequiredService<AdsDbContext>();
            });

            services.AddScoped<ILockManager, LockManager>();
            services.AddScoped<ICircuitBreakerManager, CircuitBreakerManager>();

            return services;
        }

        public static void MigrateAdsDb(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<AdsDbContext>().Database.Migrate();
            }
        }
    }
}
