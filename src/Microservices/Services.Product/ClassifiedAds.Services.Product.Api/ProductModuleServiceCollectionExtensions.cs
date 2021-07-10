using ClassifiedAds.CrossCuttingConcerns.Csv;
using ClassifiedAds.Domain.Events;
using ClassifiedAds.Domain.Repositories;
using ClassifiedAds.Infrastructure.Csv;
using ClassifiedAds.Infrastructure.Identity;
using ClassifiedAds.Services.Product.ConfigurationOptions;
using ClassifiedAds.Services.Product.Entities;
using ClassifiedAds.Services.Product.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ProductModuleServiceCollectionExtensions
    {
        public static IServiceCollection AddProductModule(this IServiceCollection services, AppSettings appSettings)
        {
            services.AddDbContext<ProductDbContext>(options => options.UseSqlServer(appSettings.ConnectionStrings.ClassifiedAds, sql =>
            {
                if (!string.IsNullOrEmpty(appSettings.ConnectionStrings.MigrationsAssembly))
                {
                    sql.MigrationsAssembly(appSettings.ConnectionStrings.MigrationsAssembly);
                }
            }));

            services
                .AddScoped<IRepository<Product, Guid>, Repository<Product, Guid>>()
                .AddScoped(typeof(IProductRepository), typeof(ProductRepository));

            DomainEvents.RegisterHandlers(Assembly.GetExecutingAssembly(), services);

            services.AddMessageHandlers(Assembly.GetExecutingAssembly());

            services.AddAuthorizationPolicies(Assembly.GetExecutingAssembly());

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ICurrentUser, CurrentWebUser>();

            services.AddScoped(typeof(ICsvReader<>), typeof(CsvReader<>));
            services.AddScoped(typeof(ICsvWriter<>), typeof(CsvWriter<>));

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
