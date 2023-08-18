using ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;
using Duende.IdentityServer;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdServerPersistenceExtensions
    {
        public static IIdentityServerBuilder AddIdServerPersistence(this IIdentityServerBuilder services, string connectionString, string migrationsAssembly = "")
        {
            services.AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = builder =>
                    builder.UseSqlServer(connectionString,
                        sql =>
                        {
                            if (!string.IsNullOrEmpty(migrationsAssembly))
                            {
                                sql.MigrationsAssembly(migrationsAssembly);
                            }
                        });
                options.DefaultSchema = "idsv";
            })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connectionString,
                            sql =>
                            {
                                if (!string.IsNullOrEmpty(migrationsAssembly))
                                {
                                    sql.MigrationsAssembly(migrationsAssembly);
                                }
                            });
                    options.DefaultSchema = "idsv";

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 30; // interval in seconds
                });
            return services;
        }

        public static void MigrateIdServerDb(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();

                var clients = new List<Client>();

                if (!context.Clients.Any(x => x.ClientId == "Swagger"))
                {
                    clients.Add(new Client
                    {
                        ClientId = "Swagger",
                        ClientName = "Swagger",
                        AllowedGrantTypes = GrantTypes.Code.Combines(GrantTypes.ClientCredentials),
                        RequirePkce = true,
                        RedirectUris =
                        {
                            "https://localhost:44312/oauth2-redirect.html",
                            "http://host.docker.internal:9002/oauth2-redirect.html",
                        },
                        AllowedScopes =
                        {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            "ClassifiedAds.WebAPI",
                        },
                        ClientSecrets =
                        {
                            new Secret("secret".Sha256()),
                        },
                        RequireConsent = false,
                    });
                }

                if (!context.Clients.Any(x => x.ClientId == "ReverseProxy.Yarp"))
                {
                    clients.Add(new Client
                    {
                        ClientId = "ReverseProxy.Yarp",
                        ClientName = "ReverseProxy Yarp",
                        AllowedGrantTypes = GrantTypes.Code.Combines(GrantTypes.ResourceOwnerPassword),
                        RequirePkce = true,
                        RedirectUris =
                        {
                            "https://localhost:44348/signin-oidc"
                        },
                        PostLogoutRedirectUris =
                        {
                            "https://localhost:44348/signout-callback-oidc"
                        },
                        AllowedScopes =
                        {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            "ClassifiedAds.WebAPI",
                        },
                        ClientSecrets =
                        {
                            new Secret("secret".Sha256()),
                        },
                        AllowOfflineAccess = true,
                        RequireConsent = true,
                    });
                }

                if (!context.Clients.Any(x => x.ClientId == "ClassifiedAds.WebMVC"))
                {
                    clients.Add(new Client
                    {
                        ClientId = "ClassifiedAds.WebMVC",
                        ClientName = "ClassifiedAds Web MVC",
                        AllowedGrantTypes = GrantTypes.Code.Combines(GrantTypes.ResourceOwnerPassword),
                        RequirePkce = true,
                        RedirectUris =
                        {
                            "https://localhost:44364/signin-oidc",
                            "http://host.docker.internal:9003/signin-oidc",
                        },
                        PostLogoutRedirectUris =
                        {
                            "https://localhost:44364/signout-callback-oidc",
                            "http://host.docker.internal:9003/signout-callback-oidc",
                        },
                        AllowedScopes =
                        {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            "ClassifiedAds.WebAPI",
                        },
                        ClientSecrets =
                        {
                            new Secret("secret".Sha256()),
                        },
                        AllowOfflineAccess = true,
                        RequireConsent = true,
                    });
                }

                if (!context.Clients.Any(x => x.ClientId == "ClassifiedAds.BlazorServerSide"))
                {
                    clients.Add(new Client
                    {
                        ClientId = "ClassifiedAds.BlazorServerSide",
                        ClientName = "ClassifiedAds BlazorServerSide",
                        AllowedGrantTypes = GrantTypes.Code.Combines(GrantTypes.ResourceOwnerPassword),
                        RequirePkce = true,
                        RedirectUris =
                        {
                            "https://localhost:44331/signin-oidc",
                            "http://host.docker.internal:9008/signin-oidc",
                        },
                        PostLogoutRedirectUris =
                        {
                            "https://localhost:44331/signout-callback-oidc",
                            "http://host.docker.internal:9008/signout-callback-oidc",
                        },
                        AllowedScopes =
                        {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            "ClassifiedAds.WebAPI",
                        },
                        ClientSecrets =
                        {
                            new Secret("secret".Sha256()),
                        },
                        AllowOfflineAccess = true,
                        RequireConsent = true,
                    });
                }

                if (!context.Clients.Any(x => x.ClientId == "ClassifiedAds.BlazorWebAssembly"))
                {
                    clients.Add(new Client
                    {
                        ClientId = "ClassifiedAds.BlazorWebAssembly",
                        ClientName = "ClassifiedAds BlazorWebAssembly",
                        AllowedGrantTypes = GrantTypes.Code,
                        RequireClientSecret = false,
                        RequirePkce = true,
                        RedirectUris =
                        {
                            "https://localhost:44348/authentication/login-callback",
                        },
                        PostLogoutRedirectUris =
                        {
                            "https://localhost:44348/authentication/logout-callback",
                        },
                        AllowedScopes =
                        {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            "ClassifiedAds.WebAPI",
                        },
                        ClientSecrets =
                        {
                            new Secret("secret".Sha256()),
                        },
                        AllowOfflineAccess = true,
                        RequireConsent = true,
                    });
                }

                if (!context.Clients.Any(x => x.ClientId == "ClassifiedAds.Angular"))
                {
                    clients.Add(new Client
                    {
                        ClientId = "ClassifiedAds.Angular",
                        ClientName = "ClassifiedAds Angular",
                        AllowedGrantTypes = GrantTypes.Code,
                        RequireClientSecret = false,
                        RequirePkce = true,
                        AllowAccessTokensViaBrowser = true,
                        RedirectUris =
                        {
                            "http://localhost:4200/oidc-login-redirect",
                        },
                        PostLogoutRedirectUris =
                        {
                            "http://localhost:4200/?postLogout=true",
                        },
                        AllowedCorsOrigins =
                        {
                            "http://localhost:4200",
                        },
                        AllowedScopes =
                        {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            "ClassifiedAds.WebAPI",
                        },
                        ClientSecrets =
                        {
                            new Secret("secret".Sha256()),
                        },
                        AllowOfflineAccess = true,
                        RequireConsent = true,
                    });
                }

                if (!context.Clients.Any(x => x.ClientId == "ClassifiedAds.React"))
                {
                    clients.Add(new Client
                    {
                        ClientId = "ClassifiedAds.React",
                        ClientName = "ClassifiedAds React",
                        AllowedGrantTypes = GrantTypes.Code,
                        RequireClientSecret = false,
                        RequirePkce = true,
                        AllowAccessTokensViaBrowser = true,
                        RedirectUris =
                        {
                            "http://localhost:3000/oidc-login-redirect",
                        },
                        PostLogoutRedirectUris =
                        {
                            "http://localhost:3000/?postLogout=true",
                        },
                        AllowedCorsOrigins =
                        {
                            "http://localhost:3000",
                        },
                        AllowedScopes =
                        {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            "ClassifiedAds.WebAPI",
                        },
                        ClientSecrets =
                        {
                            new Secret("secret".Sha256()),
                        },
                        AllowOfflineAccess = true,
                        RequireConsent = true,
                    });
                }

                if (!context.Clients.Any(x => x.ClientId == "ClassifiedAds.Vue"))
                {
                    clients.Add(new Client
                    {
                        ClientId = "ClassifiedAds.Vue",
                        ClientName = "ClassifiedAds Vue",
                        AllowedGrantTypes = GrantTypes.Code,
                        RequireClientSecret = false,
                        RequirePkce = true,
                        AllowAccessTokensViaBrowser = true,
                        RedirectUris =
                        {
                            "http://localhost:8080/oidc-login-redirect",
                        },
                        PostLogoutRedirectUris =
                        {
                            "http://localhost:8080/?postLogout=true",
                        },
                        AllowedCorsOrigins =
                        {
                            "http://localhost:8080",
                        },
                        AllowedScopes =
                        {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            "ClassifiedAds.WebAPI",
                        },
                        ClientSecrets =
                        {
                            new Secret("secret".Sha256()),
                        },
                        AllowOfflineAccess = true,
                        RequireConsent = true,
                    });
                }

                if (clients.Any())
                {
                    context.Clients.AddRange(clients.Select(x => x.ToEntity()));
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    var identityResources = new List<IdentityResource>()
                    {
                        new IdentityResources.OpenId(),
                        new IdentityResources.Profile(),
                    };

                    context.IdentityResources.AddRange(identityResources.Select(x => x.ToEntity()));

                    context.SaveChanges();
                }

                if (!context.ApiScopes.Any())
                {
                    var apiScopes = new List<ApiScope>()
                    {
                        new ApiScope("ClassifiedAds.WebAPI", "ClassifiedAds Web API"),
                    };

                    context.ApiScopes.AddRange(apiScopes.Select(x => x.ToEntity()));
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    var apiResources = new List<ApiResource>
                    {
                        new ApiResource("ClassifiedAds.WebAPI", "ClassifiedAds Web API", new[] { "role" })
                        {
                            Scopes = { "ClassifiedAds.WebAPI" },
                        },
                    };

                    context.ApiResources.AddRange(apiResources.Select(x => x.ToEntity()));

                    context.SaveChanges();
                }
            }
        }
    }
}
