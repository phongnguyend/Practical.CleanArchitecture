using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Modules.Product.Entities;
using ClassifiedAds.Modules.Product.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ProductModuleServiceCollectionExtensions
    {
        public static IServiceCollection AddProductModule(this IServiceCollection services, string connectionString, string migrationsAssembly = "")
        {
            services.AddDbContext<ProductDbContext>(options => options.UseSqlServer(connectionString, sql =>
            {
                if (!string.IsNullOrEmpty(migrationsAssembly))
                {
                    sql.MigrationsAssembly(migrationsAssembly);
                }
            }));

            services
                .AddScoped<IRepository<Product, Guid>, Repository<Product, Guid>>()
                .AddScoped(typeof(IProductRepository), typeof(ProductRepository));

            DomainEvents.RegisterHandlers(Assembly.GetExecutingAssembly(), services);

            services.AddMessageHandlers(Assembly.GetExecutingAssembly());

            return services;
        }

        public static void MigrateProductDb(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<ProductDbContext>().Database.Migrate();
            }
        }
    }
}
