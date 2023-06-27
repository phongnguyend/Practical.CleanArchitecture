using ClassifiedAds.CrossCuttingConcerns.Excel;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Services.Configuration.Authorization;
using ClassifiedAds.Services.Configuration.ConfigurationOptions;
using ClassifiedAds.Services.Configuration.Entities;
using ClassifiedAds.Services.Configuration.Excel.ClosedXML;
using ClassifiedAds.Services.Configuration.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigurationModuleServiceCollectionExtensions
{
    public static IServiceCollection AddConfigurationModule(this IServiceCollection services, AppSettings appSettings)
    {
        services.AddDbContext<ConfigurationDbContext>(options => options.UseSqlServer(appSettings.ConnectionStrings.ClassifiedAds, sql =>
        {
            if (!string.IsNullOrEmpty(appSettings.ConnectionStrings.MigrationsAssembly))
            {
                sql.MigrationsAssembly(appSettings.ConnectionStrings.MigrationsAssembly);
            }
        }))
            .AddScoped<IRepository<ConfigurationEntry, Guid>, Repository<ConfigurationEntry, Guid>>();

        services.AddMessageHandlers(Assembly.GetExecutingAssembly());

        services.AddAuthorizationPolicies(Assembly.GetExecutingAssembly(), AuthorizationPolicyNames.GetPolicyNames());

        services.AddScoped<IExcelReader<List<ConfigurationEntry>>, ConfigurationEntryExcelReader>();
        services.AddScoped<IExcelWriter<List<ConfigurationEntry>>, ConfigurationEntryExcelWriter>();

        return services;
    }

    public static IMvcBuilder AddConfigurationModule(this IMvcBuilder builder)
    {
        return builder.AddApplicationPart(Assembly.GetExecutingAssembly());
    }

    public static void MigrateConfigurationDb(this IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
        {
            serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>().Database.Migrate();
        }
    }
}
