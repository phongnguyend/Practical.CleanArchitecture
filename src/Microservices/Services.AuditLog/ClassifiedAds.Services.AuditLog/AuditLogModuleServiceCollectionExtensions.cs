using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Services.AuditLog.ConfigurationOptions;
using ClassifiedAds.Services.AuditLog.DTOs;
using ClassifiedAds.Services.AuditLog.Entities;
using ClassifiedAds.Services.AuditLog.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuditLogModuleServiceCollectionExtensions
    {
        public static IServiceCollection AddAuditLogModule(this IServiceCollection services, AppSettings appSettings)
        {
            services.AddDbContext<AuditLogDbContext>(options => options.UseSqlServer(appSettings.ConnectionStrings.ClassifiedAds, sql =>
            {
                if (!string.IsNullOrEmpty(appSettings.ConnectionStrings.MigrationsAssembly))
                {
                    sql.MigrationsAssembly(appSettings.ConnectionStrings.MigrationsAssembly);
                }
            }))
                .AddScoped<IRepository<AuditLogEntry, Guid>, Repository<AuditLogEntry, Guid>>();

            services.AddMessageHandlers(Assembly.GetExecutingAssembly());

            services.AddAuthorizationPolicies(Assembly.GetExecutingAssembly());

            services.AddMessageBusReceiver<AuditLogCreatedEvent>(appSettings.MessageBroker);

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
