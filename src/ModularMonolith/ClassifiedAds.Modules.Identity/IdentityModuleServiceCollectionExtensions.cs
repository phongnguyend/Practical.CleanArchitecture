using ClassifiedAds.Modules.Identity;
using ClassifiedAds.Modules.Identity.Contracts.Services;
using ClassifiedAds.Modules.Identity.Entities;
using ClassifiedAds.Modules.Identity.Repositories;
using ClassifiedAds.Modules.Identity.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityModuleServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentityModule(this IServiceCollection services, string connectionString, string migrationsAssembly = "")
        {
            services.AddDbContext<IdentityDbContext>(options => options.UseSqlServer(connectionString, sql =>
            {
                if (!string.IsNullOrEmpty(migrationsAssembly))
                {
                    sql.MigrationsAssembly(migrationsAssembly);
                }
            }))
                .AddScoped(typeof(IUserRepository), typeof(UserRepository))
                .AddScoped(typeof(IRoleRepository), typeof(RoleRepository))
                .AddScoped(typeof(IUserService), typeof(UserService));

            services.AddIdentity<User, Role>(options =>
                    {
                        options.Tokens.EmailConfirmationTokenProvider = "EmailConfirmation";
                    })
                    .AddUserManager<UserManager<User>>()
                    .AddDefaultTokenProviders()
                    .AddTokenProvider<EmailConfirmationTokenProvider<User>>("EmailConfirmation");
            services.AddTransient<IUserStore<User>, UserStore>();
            services.AddTransient<IRoleStore<Role>, RoleStore>();

            services.Configure<DataProtectionTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromHours(3));
            services.Configure<EmailConfirmationTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromDays(2));

            services.AddMessageHandlers(Assembly.GetExecutingAssembly());

            return services;
        }

        public static IServiceCollection AddIdentityModuleCore(this IServiceCollection services, string connectionString, string migrationsAssembly = "")
        {
            services.AddDbContext<IdentityDbContext>(options => options.UseSqlServer(connectionString, sql =>
            {
                if (!string.IsNullOrEmpty(migrationsAssembly))
                {
                    sql.MigrationsAssembly(migrationsAssembly);
                }
            }))
                .AddScoped(typeof(IUserRepository), typeof(UserRepository))
                .AddScoped(typeof(IRoleRepository), typeof(RoleRepository))
                .AddScoped(typeof(IUserService), typeof(UserService));

            services.AddIdentityCore<User>(options =>
                    {
                        options.Tokens.EmailConfirmationTokenProvider = "EmailConfirmation";
                    })
                    .AddUserManager<UserManager<User>>()
                    .AddDefaultTokenProviders()
                    .AddTokenProvider<EmailConfirmationTokenProvider<User>>("EmailConfirmation");
            services.AddTransient<IUserStore<User>, UserStore>();
            services.AddTransient<IRoleStore<Role>, RoleStore>();

            services.Configure<DataProtectionTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromHours(3));
            services.Configure<EmailConfirmationTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromDays(2));

            services.AddMessageHandlers(Assembly.GetExecutingAssembly());

            return services;
        }

        public static void MigrateIdentityDb(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<IdentityDbContext>().Database.Migrate();
            }
        }
    }
}
