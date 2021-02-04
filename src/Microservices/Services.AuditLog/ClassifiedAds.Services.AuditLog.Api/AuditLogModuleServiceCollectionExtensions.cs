﻿using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Services.AuditLog.Entities;
using ClassifiedAds.Services.AuditLog.Repositories;
using ClassifiedAds.Services.AuditLog.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuditLogModuleServiceCollectionExtensions
    {
        public static IServiceCollection AddAuditLogModule(this IServiceCollection services, string connectionString, string migrationsAssembly = "")
        {
            services.AddDbContext<AuditLogDbContext>(options => options.UseSqlServer(connectionString, sql =>
            {
                if (!string.IsNullOrEmpty(migrationsAssembly))
                {
                    sql.MigrationsAssembly(migrationsAssembly);
                }
            }))
                .AddScoped<IRepository<AuditLogEntry, Guid>, Repository<AuditLogEntry, Guid>>()
                .AddScoped(typeof(IAuditLogService), typeof(AuditLogService));

            services.AddMessageHandlers(Assembly.GetExecutingAssembly());

            services.AddAuthorizationPolicies(Assembly.GetExecutingAssembly());

            return services;
        }

        public static void MigrateAuditLogDb(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<AuditLogDbContext>().Database.Migrate();
            }
        }
    }
}
