using ClassifiedAds.Modules.Identity;
using ClassifiedAds.Modules.Identity.ConfigurationOptions;
using ClassifiedAds.Modules.Identity.Contracts.Services;
using ClassifiedAds.Modules.Identity.Entities;
using ClassifiedAds.Modules.Identity.PasswordValidators;
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
        public static IServiceCollection AddIdentityModule(this IServiceCollection services, IdentityModuleOptions moduleOptions)
        {
            services.Configure<IdentityModuleOptions>(op =>
            {
                op.ConnectionStrings = moduleOptions.ConnectionStrings;
                op.IdentityServerAuthentication = moduleOptions.IdentityServerAuthentication;
            });

            services.AddDbContext<IdentityDbContext>(options => options.UseSqlServer(moduleOptions.ConnectionStrings.Default, sql =>
            {
                if (!string.IsNullOrEmpty(moduleOptions.ConnectionStrings.MigrationsAssembly))
                {
                    sql.MigrationsAssembly(moduleOptions.ConnectionStrings.MigrationsAssembly);
                }
            }))
                .AddScoped(typeof(IUserRepository), typeof(UserRepository))
                .AddScoped(typeof(IRoleRepository), typeof(RoleRepository))
                .AddScoped(typeof(IUserService), typeof(UserService));

            services.AddIdentity<User, Role>()
                    .AddTokenProviders()
                    .AddPasswordValidators();

            services.AddTransient<IUserStore<User>, UserStore>();
            services.AddTransient<IRoleStore<Role>, RoleStore>();

            ConfigureOptions(services);

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
            });

            services.AddMessageHandlers(Assembly.GetExecutingAssembly());

            return services;
        }

        public static IServiceCollection AddIdentityModuleCore(this IServiceCollection services, IdentityModuleOptions moduleOptions)
        {
            services.Configure<IdentityModuleOptions>(op =>
            {
                op.ConnectionStrings = moduleOptions.ConnectionStrings;
                op.IdentityServerAuthentication = moduleOptions.IdentityServerAuthentication;
            });

            services.AddDbContext<IdentityDbContext>(options => options.UseSqlServer(moduleOptions.ConnectionStrings.Default, sql =>
            {
                if (!string.IsNullOrEmpty(moduleOptions.ConnectionStrings.MigrationsAssembly))
                {
                    sql.MigrationsAssembly(moduleOptions.ConnectionStrings.MigrationsAssembly);
                }
            }))
                .AddScoped(typeof(IUserRepository), typeof(UserRepository))
                .AddScoped(typeof(IRoleRepository), typeof(RoleRepository))
                .AddScoped(typeof(IUserService), typeof(UserService));

            services.AddIdentityCore<User>()
                    .AddTokenProviders()
                    .AddPasswordValidators();

            services.AddTransient<IUserStore<User>, UserStore>();
            services.AddTransient<IRoleStore<Role>, RoleStore>();

            ConfigureOptions(services);

            services.AddMessageHandlers(Assembly.GetExecutingAssembly());

            return services;
        }

        private static IdentityBuilder AddTokenProviders(this IdentityBuilder identityBuilder)
        {
            identityBuilder
                .AddDefaultTokenProviders()
                .AddTokenProvider<EmailConfirmationTokenProvider<User>>("EmailConfirmation");

            return identityBuilder;
        }

        private static IdentityBuilder AddPasswordValidators(this IdentityBuilder identityBuilder)
        {
            identityBuilder
                .AddPasswordValidator<WeakPasswordValidator>()
                .AddPasswordValidator<HistoricalPasswordValidator>();

            return identityBuilder;
        }

        private static void ConfigureOptions(IServiceCollection services)
        {
            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(3);
            });

            services.Configure<EmailConfirmationTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromDays(2);
            });

            services.Configure<IdentityOptions>(options =>
            {
                options.Tokens.EmailConfirmationTokenProvider = "EmailConfirmation";

                // Default Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            });

            services.Configure<PasswordHasherOptions>(option =>
            {
                // option.IterationCount = 10000;
                // option.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV2;
            });

            services.AddAuthorizationPolicies(Assembly.GetExecutingAssembly());
        }

        public static IMvcBuilder AddIdentityModule(this IMvcBuilder builder)
        {
            return builder.AddApplicationPart(Assembly.GetExecutingAssembly());
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
