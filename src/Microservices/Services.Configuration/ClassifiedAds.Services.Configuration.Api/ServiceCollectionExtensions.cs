using ClassifiedAds.CrossCuttingConcerns.Excel;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Services.Configuration.ConfigurationOptions;
using ClassifiedAds.Services.Configuration.Entities;
using ClassifiedAds.Services.Configuration.Excel;
using ClassifiedAds.Services.Configuration.Excel.ClosedXML;
using ClassifiedAds.Services.Configuration.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConfigurationModule(this IServiceCollection services, AppSettings appSettings)
    {
        services.AddCaches(appSettings.Caching);

        services.AddDbContext<ConfigurationDbContext>(options => options.UseSqlServer(appSettings.ConnectionStrings.ClassifiedAds, sql =>
        {
            if (!string.IsNullOrEmpty(appSettings.ConnectionStrings.MigrationsAssembly))
            {
                sql.MigrationsAssembly(appSettings.ConnectionStrings.MigrationsAssembly);
            }

            if (appSettings.ConnectionStrings.CommandTimeout.HasValue)
            {
                sql.CommandTimeout(appSettings.ConnectionStrings.CommandTimeout);
            }
        }))
            .AddScoped<IRepository<ConfigurationEntry, Guid>, Repository<ConfigurationEntry, Guid>>();

        services.AddMessageHandlers(Assembly.GetExecutingAssembly());

        services.AddAuthorizationPolicies(Assembly.GetExecutingAssembly());

        services.AddScoped<IExcelReader<ImportConfigurationEntriesFromExcel>, ImportConfigurationEntriesFromExcelHandler>();
        services.AddScoped<IExcelWriter<ExportConfigurationEntriesToExcel>, ExportConfigurationEntriesToExcelHandler>();

        return services;
    }

    public static IMvcBuilder AddConfigurationModule(this IMvcBuilder builder)
    {
        return builder.AddApplicationPart(Assembly.GetExecutingAssembly());
    }

    public static void MigrateConfigurationDb(this IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>().Database.Migrate();
        }
    }
}
