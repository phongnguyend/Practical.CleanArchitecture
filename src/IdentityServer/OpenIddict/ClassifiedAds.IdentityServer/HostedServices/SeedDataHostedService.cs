using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.IdentityServer.HostedServices
{
    public class SeedDataHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public SeedDataHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            if (await manager.FindByClientIdAsync("ClassifiedAds.WebMVC", cancellationToken) is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "ClassifiedAds.WebMVC",
                    ClientSecret = "secret",
                    DisplayName = "ClassifiedAds Web MVC",
                    RedirectUris = { new Uri("https://localhost:44364/signin-oidc") },
                    PostLogoutRedirectUris = { new Uri("https://localhost:44364/signout-callback-oidc") },
                    Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Authorization,
                        OpenIddictConstants.Permissions.Endpoints.Token,
                        OpenIddictConstants.Permissions.Endpoints.Logout,

                        OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                        OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                        OpenIddictConstants.Permissions.GrantTypes.RefreshToken,

                        OpenIddictConstants.Permissions.Prefixes.Scope + "openid",
                        OpenIddictConstants.Permissions.Scopes.Profile,
                        OpenIddictConstants.Permissions.Prefixes.Scope + "ClassifiedAds.WebAPI",
                        OpenIddictConstants.Permissions.Prefixes.Scope + "offline_access",
                        OpenIddictConstants.Permissions.ResponseTypes.Code
                    },
                    Requirements =
                    {
                        OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange
                    }
                }, cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
