using ClassifiedAds.CrossCuttingConcerns.Excel;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Modules.Configuration.ConfigurationOptions;
using ClassifiedAds.Modules.Configuration.Entities;
using ClassifiedAds.Modules.Configuration.Excel;
using ClassifiedAds.Modules.Configuration.Excel.ClosedXML;
using ClassifiedAds.Modules.Configuration.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConfigurationModule(this IServiceCollection services, Action<ConfigurationModuleOptions> configureOptions)
    {
        var settings = new ConfigurationModuleOptions();
        configureOptions(settings);

        services.Configure(configureOptions);

        services.AddDbContext<ConfigurationDbContext>(options => options.UseSqlServer(settings.ConnectionStrings.Default, sql =>
        {
            if (!string.IsNullOrEmpty(settings.ConnectionStrings.MigrationsAssembly))
            {
                sql.MigrationsAssembly(settings.ConnectionStrings.MigrationsAssembly);
            }

            if (settings.ConnectionStrings.CommandTimeout.HasValue)
            {
                sql.CommandTimeout(settings.ConnectionStrings.CommandTimeout);
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
        using var serviceScope = app.ApplicationServices.CreateScope();
        serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>().Database.Migrate();
    }

    public static void MigrateConfigurationDb(this IHost app)
    {
        using var serviceScope = app.Services.CreateScope();
        serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>().Database.Migrate();
    }
}
