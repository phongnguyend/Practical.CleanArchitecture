using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Domain.Identity;
using ClassifiedAds.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System;

namespace Microsoft.Extensions.DependencyInjection;

public static class IdentityServiceCollectionExtensions
{
    public static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services.AddIdentity<User, Role>()
                .AddTokenProviders()
                .AddPasswordValidators();

        services.AddTransient<IUserStore<User>, UserStore>();
        services.AddTransient<IRoleStore<Role>, RoleStore>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        ConfigureOptions(services);

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
        });

        return services;
    }

    public static IServiceCollection AddIdentityCore(this IServiceCollection services)
    {
        services.AddIdentityCore<User>()
                .AddTokenProviders()
                .AddPasswordValidators();

        services.AddTransient<IUserStore<User>, UserStore>();
        services.AddTransient<IRoleStore<Role>, RoleStore>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        ConfigureOptions(services);

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
    }
}
