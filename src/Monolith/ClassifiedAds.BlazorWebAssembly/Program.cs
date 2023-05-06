using ClassifiedAds.Blazor.Modules.AuditLogs.Services;
using ClassifiedAds.Blazor.Modules.Core.Services;
using ClassifiedAds.Blazor.Modules.Files.Services;
using ClassifiedAds.Blazor.Modules.Products.Services;
using ClassifiedAds.Blazor.Modules.Settings.Services;
using ClassifiedAds.Blazor.Modules.Users.Services;
using ClassifiedAds.BlazorWebAssembly.ConfigurationOptions;
using ClassifiedAds.BlazorWebAssembly.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClassifiedAds.BlazorWebAssembly;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("app");

        var appSettings = new AppSettings();
        builder.Configuration.Bind(appSettings);

        builder.Services.AddOidcAuthentication(options =>
        {
            options.ProviderOptions.Authority = appSettings.OpenIdConnect.Authority;
            options.ProviderOptions.ClientId = appSettings.OpenIdConnect.ClientId;
            options.ProviderOptions.RedirectUri = appSettings.OpenIdConnect.RedirectUri;
            options.ProviderOptions.PostLogoutRedirectUri = appSettings.OpenIdConnect.PostLogoutRedirectUri;
            options.ProviderOptions.ResponseType = "code";
            options.ProviderOptions.DefaultScopes.Add("openid");
            options.ProviderOptions.DefaultScopes.Add("profile");
            options.ProviderOptions.DefaultScopes.Add("ClassifiedAds.WebAPI");
            options.UserOptions.NameClaim = "name";
        });

        builder.Services.AddAuthorizationCore();

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        builder.Services.AddScoped<ITokenManager, TokenManager>();

        builder.Services.AddHttpClient<FileService, FileService>(client =>
        {
            client.BaseAddress = new Uri(appSettings.ResourceServer.Endpoint);
            client.Timeout = new TimeSpan(0, 0, 30);
            client.DefaultRequestHeaders.Clear();
        })
        .AddHttpMessageHandler(sp =>
        {
            var handler = sp.GetService<AuthorizationMessageHandler>()
            .ConfigureHandler(
                authorizedUrls: new[] { appSettings.ResourceServer.Endpoint }
             );
            return handler;
        });

        builder.Services.AddHttpClient<ConfigurationEntryService, ConfigurationEntryService>(client =>
        {
            client.BaseAddress = new Uri(appSettings.ResourceServer.Endpoint);
            client.Timeout = new TimeSpan(0, 0, 30);
            client.DefaultRequestHeaders.Clear();
        })
        .AddHttpMessageHandler(sp =>
        {
            var handler = sp.GetService<AuthorizationMessageHandler>()
            .ConfigureHandler(
                authorizedUrls: new[] { appSettings.ResourceServer.Endpoint }
             );
            return handler;
        });

        builder.Services.AddHttpClient<ProductService, ProductService>(client =>
        {
            client.BaseAddress = new Uri(appSettings.ResourceServer.Endpoint);
            client.Timeout = new TimeSpan(0, 0, 30);
            client.DefaultRequestHeaders.Clear();
        })
        .AddHttpMessageHandler(sp =>
        {
            var handler = sp.GetService<AuthorizationMessageHandler>()
            .ConfigureHandler(
                authorizedUrls: new[] { appSettings.ResourceServer.Endpoint }
                );
            return handler;
        });

        builder.Services.AddHttpClient<UserService, UserService>(client =>
        {
            client.BaseAddress = new Uri(appSettings.ResourceServer.Endpoint);
            client.Timeout = new TimeSpan(0, 0, 30);
            client.DefaultRequestHeaders.Clear();
        })
            .AddHttpMessageHandler(sp =>
        {
            var handler = sp.GetService<AuthorizationMessageHandler>()
            .ConfigureHandler(
                authorizedUrls: new[] { appSettings.ResourceServer.Endpoint }
             );
            return handler;
        });

        builder.Services.AddHttpClient<AuditLogService, AuditLogService>(client =>
        {
            client.BaseAddress = new Uri(appSettings.ResourceServer.Endpoint);
            client.Timeout = new TimeSpan(0, 0, 30);
            client.DefaultRequestHeaders.Clear();
        })
        .AddHttpMessageHandler(sp =>
        {
            var handler = sp.GetService<AuthorizationMessageHandler>()
            .ConfigureHandler(
                authorizedUrls: new[] { appSettings.ResourceServer.Endpoint }
                );
            return handler;
        });

        await builder.Build().RunAsync();
    }
}
