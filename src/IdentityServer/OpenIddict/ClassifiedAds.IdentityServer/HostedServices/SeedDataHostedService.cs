using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.IdentityServer.HostedServices;

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

        await UpsertClientApplication(manager, new OpenIddictApplicationDescriptor
        {
            ClientId = "Swagger",
            ClientSecret = "secret",
            DisplayName = "Swagger",
            RedirectUris =
            {
                new Uri("https://localhost:44312/oauth2-redirect.html")
            },
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
            },
            Type = OpenIddictConstants.ClientTypes.Confidential,
        }, cancellationToken);

        await UpsertClientApplication(manager, new OpenIddictApplicationDescriptor
        {
            ClientId = "ReverseProxy.Yarp",
            ClientSecret = "secret",
            DisplayName = "ReverseProxy Yarp",
            RedirectUris =
            {
                new Uri("https://localhost:44348/signin-oidc")
            },
            PostLogoutRedirectUris =
            {
                new Uri("https://localhost:44348/signout-callback-oidc")
            },
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.Endpoints.Logout,

                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                OpenIddictConstants.Permissions.GrantTypes.Password,
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
            },
            Type = OpenIddictConstants.ClientTypes.Confidential,
        }, cancellationToken);

        await UpsertClientApplication(manager, new OpenIddictApplicationDescriptor
        {
            ClientId = "ClassifiedAds.WebMVC",
            ClientSecret = "secret",
            DisplayName = "ClassifiedAds Web MVC",
            RedirectUris =
            {
                new Uri("https://localhost:44364/signin-oidc")
            },
            PostLogoutRedirectUris =
            {
                new Uri("https://localhost:44364/signout-callback-oidc")
            },
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.Endpoints.Logout,

                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                OpenIddictConstants.Permissions.GrantTypes.Password,
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
            },
            Type = OpenIddictConstants.ClientTypes.Confidential,
        }, cancellationToken);

        await UpsertClientApplication(manager, new OpenIddictApplicationDescriptor
        {
            ClientId = "ClassifiedAds.BlazorServerSide",
            ClientSecret = "secret",
            DisplayName = "ClassifiedAds BlazorServerSide",
            RedirectUris =
            {
                new Uri("https://localhost:44331/signin-oidc")
            },
            PostLogoutRedirectUris =
            {
                new Uri("https://localhost:44331/signout-callback-oidc")
            },
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
            },
            Type = OpenIddictConstants.ClientTypes.Confidential,
        }, cancellationToken);

        await UpsertClientApplication(manager, new OpenIddictApplicationDescriptor
        {
            ClientId = "ClassifiedAds.BlazorWebAssembly",
            DisplayName = "ClassifiedAds BlazorWebAssembly",
            RedirectUris =
            {
                new Uri("https://localhost:44348/authentication/login-callback")
            },
            PostLogoutRedirectUris =
            {
                new Uri("https://localhost:44348/authentication/logout-callback")
            },
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
            },
            Type = OpenIddictConstants.ClientTypes.Public,
        }, cancellationToken);

        await UpsertClientApplication(manager, new OpenIddictApplicationDescriptor
        {
            ClientId = "ClassifiedAds.Angular",
            DisplayName = "ClassifiedAds Angular",
            RedirectUris =
            {
                new Uri("http://localhost:4200/oidc-login-redirect")
            },
            PostLogoutRedirectUris =
            {
                new Uri("http://localhost:4200/?postLogout=true")
            },
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
            },
            Type = OpenIddictConstants.ClientTypes.Public,
        }, cancellationToken);

        await UpsertClientApplication(manager, new OpenIddictApplicationDescriptor
        {
            ClientId = "ClassifiedAds.React",
            DisplayName = "ClassifiedAds React",
            RedirectUris =
            {
                new Uri("http://localhost:3000/oidc-login-redirect")
            },
            PostLogoutRedirectUris =
            {
                new Uri("http://localhost:3000/?postLogout=true")
            },
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
            },
            Type = OpenIddictConstants.ClientTypes.Public,
        }, cancellationToken);

        await UpsertClientApplication(manager, new OpenIddictApplicationDescriptor
        {
            ClientId = "ClassifiedAds.Vue",
            DisplayName = "ClassifiedAds Vue",
            RedirectUris =
            {
                new Uri("http://localhost:8080/oidc-login-redirect")
            },
            PostLogoutRedirectUris =
            {
                new Uri("http://localhost:8080/?postLogout=true")
            },
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
            },
            Type = OpenIddictConstants.ClientTypes.Public,
        }, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static async Task UpsertClientApplication(IOpenIddictApplicationManager manager, OpenIddictApplicationDescriptor openIddictApplicationDescriptor, CancellationToken cancellationToken)
    {
        var client = await manager.FindByClientIdAsync(openIddictApplicationDescriptor.ClientId, cancellationToken);

        if (client is null)
        {
            await manager.CreateAsync(openIddictApplicationDescriptor, cancellationToken);
        }
        else
        {
            await manager.UpdateAsync(client, openIddictApplicationDescriptor, cancellationToken);
        }
    }
}
