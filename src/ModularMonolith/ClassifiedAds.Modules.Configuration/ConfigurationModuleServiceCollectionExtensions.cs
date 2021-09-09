using ClassifiedAds.CrossCuttingConcerns.Excel;
using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Modules.Configuration.ConfigurationOptions;
using ClassifiedAds.Modules.Configuration.Entities;
using ClassifiedAds.Modules.Configuration.Excel.ClosedXML;
using ClassifiedAds.Modules.Configuration.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ConfigurationModuleServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigurationModule(this IServiceCollection services, ConfigurationModuleOptions moduleOptions)
        {
            services.Configure<ConfigurationModuleOptions>(op =>
            {
                op.ConnectionStrings = moduleOptions.ConnectionStrings;
                op.Certificates = moduleOptions.Certificates;
            });

            services.AddDbContext<ConfigurationDbContext>(options => options.UseSqlServer(moduleOptions.ConnectionStrings.Default, sql =>
            {
                if (!string.IsNullOrEmpty(moduleOptions.ConnectionStrings.MigrationsAssembly))
                {
                    sql.MigrationsAssembly(moduleOptions.ConnectionStrings.MigrationsAssembly);
                }
            }))
                .AddScoped<IRepository<ConfigurationEntry, Guid>, Repository<ConfigurationEntry, Guid>>();

            DomainEvents.RegisterHandlers(Assembly.GetExecutingAssembly(), services);

            services.AddMessageHandlers(Assembly.GetExecutingAssembly());

            services.AddAuthorizationPolicies(Assembly.GetExecutingAssembly());

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
}
