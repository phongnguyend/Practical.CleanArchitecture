using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using ReverseProxy.Yarp.ConfigurationOptions;
using System.Net;
using System.Net.Http.Headers;
using Yarp.ReverseProxy.Transforms;

namespace Practical.ReverseProxy.Yarp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;
            var configuration = builder.Configuration;

            var appSettings = new AppSettings();
            configuration.Bind(appSettings);

            // Add the reverse proxy to capability to the server
            var proxyBuilder = builder.Services.AddReverseProxy();

            // Initialize the reverse proxy from the "ReverseProxy" section of configuration
            proxyBuilder.LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
                .AddTransforms(transformBuilderContext =>
                {
                    transformBuilderContext.AddRequestTransform(async transformContext =>
                    {
                        var user = transformContext.HttpContext.User;
                        var token = await transformContext.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
                        transformContext.ProxyRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    });

                });

            services.AddControllers();

            services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.AccessDeniedPath = "/Authorization/AccessDenied";
            })
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = appSettings.OpenIdConnect?.Authority;
                options.ClientId = appSettings.OpenIdConnect?.ClientId;
                options.ClientSecret = appSettings.OpenIdConnect?.ClientSecret;
                options.ResponseType = "code";
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("ClassifiedAds.WebAPI");
                options.Scope.Add("offline_access");
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.RequireHttpsMetadata = appSettings.OpenIdConnect?.RequireHttpsMetadata ?? false;
            });

            var app = builder.Build();

            app.MapControllers();

            app.Use(async (context, next) =>
            {
                if (context.Request.Path.Value?.StartsWith("/api/", StringComparison.OrdinalIgnoreCase) ?? false)
                {
                    try
                    {
                        var antiForgeryService = context.RequestServices.GetRequiredService<IAntiforgery>();
                        await antiForgeryService.ValidateRequestAsync(context);
                    }
                    catch (AntiforgeryValidationException)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        return;
                    }
                }

                await next(context);
            });

            app.MapReverseProxy();

            app.MapForwarder("{**rest}", "http://localhost:3000");

            app.Run();
        }
    }
}