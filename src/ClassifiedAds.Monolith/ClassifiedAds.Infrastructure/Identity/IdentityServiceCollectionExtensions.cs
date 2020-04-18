using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentity(this IServiceCollection services)
        {
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
            return services;
        }

        public static IServiceCollection AddIdentityCore(this IServiceCollection services)
        {
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
            return services;
        }
    }
}
