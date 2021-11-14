using ClassifiedAds.CrossCuttingConcerns.Locks;
using ClassifiedAds.CrossCuttingConcerns.Tenants;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Persistence;
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
                    .AddRepositories();
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
            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>))
                    .AddScoped(typeof(IAuditLogEntryRepository), typeof(AuditLogEntryRepository))
                    .AddScoped(typeof(IEmailMessageRepository), typeof(EmailMessageRepository))
                    .AddScoped(typeof(ISmsMessageRepository), typeof(SmsMessageRepository))
                    .AddScoped(typeof(IUserRepository), typeof(UserRepository))
                    .AddScoped(typeof(IRoleRepository), typeof(RoleRepository));

            services.AddScoped<ILockManager, LockManager>();

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
