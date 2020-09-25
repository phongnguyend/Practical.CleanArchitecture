using System.Collections.Generic;
using System.Linq;
using ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdServerPersistenceExtensions
    {
        public static IIdentityServerBuilder AddTokenProviderModule(this IIdentityServerBuilder services, string connectionString, string migrationsAssembly = "")
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
                if (!context.Clients.Any(x => x.ClientId == "ClassifiedAds.WebMVC"))
                {
                    clients.Add(new Client
                    {
                        ClientId = "ClassifiedAds.WebMVC",
                        ClientName = "ClassifiedAds Web MVC",
                        AllowedGrantTypes = GrantTypes.Hybrid.Combines(GrantTypes.ResourceOwnerPassword),
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
                    });
                }

                if (!context.Clients.Any(x => x.ClientId == "ClassifiedAds.Blazor"))
                {
                    clients.Add(new Client
                    {
                        ClientId = "ClassifiedAds.Blazor",
                        ClientName = "ClassifiedAds Blazor",
                        AllowedGrantTypes = GrantTypes.Hybrid.Combines(GrantTypes.ResourceOwnerPassword),
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
                    });
                }

                if (!context.Clients.Any(x => x.ClientId == "ClassifiedAds.Angular"))
                {
                    clients.Add(new Client
                    {
                        ClientId = "ClassifiedAds.Angular",
                        ClientName = "ClassifiedAds Angular",
                        AllowedGrantTypes = GrantTypes.Implicit,
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
                    });
                }

                if (!context.Clients.Any(x => x.ClientId == "ClassifiedAds.React"))
                {
                    clients.Add(new Client
                    {
                        ClientId = "ClassifiedAds.React",
                        ClientName = "ClassifiedAds React",
                        AllowedGrantTypes = GrantTypes.Implicit,
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
                    });
                }

                if (!context.Clients.Any(x => x.ClientId == "ClassifiedAds.Vue"))
                {
                    clients.Add(new Client
                    {
                        ClientId = "ClassifiedAds.Vue",
                        ClientName = "ClassifiedAds Vue",
                        AllowedGrantTypes = GrantTypes.Implicit,
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

                if (!context.ApiResources.Any())
                {
                    var apiResources = new List<ApiResource>
                    {
                        new ApiResource("ClassifiedAds.WebAPI", "ClassifiedAds Web API",
                        new List<string>() { "role" }),
                    };

                    context.ApiResources.AddRange(apiResources.Select(x => x.ToEntity()));

                    context.SaveChanges();
                }
            }
        }
    }
}
