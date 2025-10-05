using ClassifiedAds.CrossCuttingConcerns.Tenants;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Persistence;
using ClassifiedAds.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System;

namespace Microsoft.Extensions.DependencyInjection;

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
                .AddScoped<IAuditLogEntryRepository, AuditLogEntryRepository>()
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IRoleRepository, RoleRepository>();

        services.AddScoped(typeof(IUnitOfWork), services =>
        {
            return services.GetRequiredService<AdsDbContext>();
        });

        return services;
    }
}
