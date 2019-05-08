using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;

namespace ClassifiedAds.IdentityServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddTestUsers(TestUsers.Users)
                .AddInMemoryIdentityResources(new List<IdentityResource>()
                {
                    new IdentityResources.OpenId(),
                    new IdentityResources.Profile(),
                })
                .AddInMemoryApiResources(new List<ApiResource> {
                    new ApiResource("ClassifiedAds.WebAPI", "ClassifiedAds Web API",
                    new List<string>() {"role" } )
                })
                .AddInMemoryClients(new List<Client> {
                    new Client
                    {
                        ClientId = "ClassifiedAds.WebMVC",
                        ClientName ="ClassifiedAds Web MVC",
                        AllowedGrantTypes = GrantTypes.Hybrid.Combines(GrantTypes.ResourceOwnerPassword),
                        RedirectUris =
                        {
                            "https://localhost:44364/signin-oidc"
                        },
                        PostLogoutRedirectUris =
                        {
                            "https://localhost:44364/signout-callback-oidc"
                        },
                        AllowedScopes =
                        {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            "ClassifiedAds.WebAPI"
                        },
                        ClientSecrets =
                        {
                            new Secret("secret".Sha256())
                        }
                    }
                });

            services.AddMiniProfiler(options =>
            {
                options.RouteBasePath = "/profiler";// access /profiler/results to see last profile check
                options.PopupRenderPosition = StackExchange.Profiling.RenderPosition.BottomLeft;
                options.PopupShowTimeWithChildren = true;
            })
            .AddEntityFramework();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiniProfiler();
            app.UseIdentityServer();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
