using ClassifiedAds.Infrastructure.Monitoring;
using ClassifiedAds.Infrastructure.Notification;
using ClassifiedAds.Infrastructure.Notification.Email;
using ClassifiedAds.Infrastructure.Notification.Sms;
using ClassifiedAds.Infrastructure.Notification.Web;
using ClassifiedAds.Infrastructure.Web.Filters;
using ClassifiedAds.Modules.Configuration.ConfigurationOptions;
using ClassifiedAds.Modules.Identity.ConfigurationOptions;
using ClassifiedAds.Modules.Identity.Contracts.Services;
using ClassifiedAds.Modules.Identity.Repositories;
using ClassifiedAds.Modules.Identity.Services;
using ClassifiedAds.Modules.Notification.Hubs;
using ClassifiedAds.WebAPI.ConfigurationOptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;

namespace ClassifiedAds.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;

            AppSettings = new AppSettings();
            Configuration.Bind(AppSettings);
        }

        public IConfiguration Configuration { get; }
        private AppSettings AppSettings { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration);

            services.AddControllers(configure =>
            {
                configure.Filters.Add(typeof(GlobalExceptionFilter));
            })
                .AddAuditLogModule()
                .AddConfigurationModule()
                .AddIdentityModule()
                .AddNotificationModule()
                .AddProductModule()
                .AddStorageModule()
                .AddMonitoringServices(AppSettings.Monitoring);

            services.AddSignalR();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowedOrigins", builder => builder
                    .WithOrigins(AppSettings.CORS.AllowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader());

                options.AddPolicy("SignalRHubs", builder => builder
                    .WithOrigins(AppSettings.CORS.AllowedOrigins)
                    .AllowAnyHeader()
                    .WithMethods("GET", "POST")
                    .AllowCredentials());

                options.AddPolicy("AllowAnyOrigin", builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());

                options.AddPolicy("CustomPolicy", builder => builder
                    .AllowAnyOrigin()
                    .WithMethods("Get")
                    .WithHeaders("Content-Type"));
            });

            services.AddDateTimeProvider();

            var notificationOptions = new NotificationOptions
            {
                Email = new EmailOptions { Provider = "Fake" },
                Sms = new SmsOptions { Provider = "Fake" },
                Web = new WebOptions { Provider = "Fake" },
            };

            services.AddAuditLogModule(new Modules.AuditLog.ConfigurationOptions.AuditLogModuleOptions
            {
                ConnectionStrings = new Modules.AuditLog.ConfigurationOptions.ConnectionStringsOptions
                {
                    Default = AppSettings.ConnectionStrings.ClassifiedAds,
                },
            })
            .AddConfigurationModule(new ConfigurationModuleOptions
            {
                ConnectionStrings = new Modules.Configuration.ConfigurationOptions.ConnectionStringsOptions
                {
                    Default = AppSettings.ConnectionStrings.ClassifiedAds,
                },
                Certificates = AppSettings.ConfigurationModule.Certificates,
            })
            .AddIdentityModuleCore(new IdentityModuleOptions
            {
                ConnectionStrings = new Modules.Identity.ConfigurationOptions.ConnectionStringsOptions
                {
                    Default = AppSettings.ConnectionStrings.ClassifiedAds,
                },
                IdentityServerAuthentication = new Modules.Identity.ConfigurationOptions.IdentityServerAuthentication
                {
                    Authority = AppSettings.IdentityServerAuthentication.Authority,
                },
            })
            .AddNotificationModule(new Modules.Notification.ConfigurationOptions.NotificationModuleOptions
            {
                ConnectionStrings = new Modules.Notification.ConfigurationOptions.ConnectionStringsOptions
                {
                    Default = AppSettings.ConnectionStrings.ClassifiedAds,
                },
                MessageBroker = AppSettings.MessageBroker,
                Notification = notificationOptions,
            })
            .AddProductModule(new Modules.Product.ConfigurationOptions.ProductModuleOptions
            {
                ConnectionStrings = new Modules.Product.ConfigurationOptions.ConnectionStringsOptions
                {
                    Default = AppSettings.ConnectionStrings.ClassifiedAds,
                },
            })
            .AddStorageModule(new Modules.Storage.ConfigurationOptions.StorageModuleOptions
            {
                ConnectionStrings = new Modules.Storage.ConfigurationOptions.ConnectionStringsOptions
                {
                    Default = AppSettings.ConnectionStrings.ClassifiedAds,
                },
                Storage = AppSettings.Storage,
                MessageBroker = AppSettings.MessageBroker,
            })
            .AddApplicationServices();

            services.AddHtmlGenerator();
            services.AddDinkToPdfConverter();

            services.AddDataProtection()
                .PersistKeysToDbContext<IdentityDbContext>()
                .SetApplicationName("ClassifiedAds");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = AppSettings.IdentityServerAuthentication.Authority;
                    options.Audience = AppSettings.IdentityServerAuthentication.ApiName;
                    options.RequireHttpsMetadata = AppSettings.IdentityServerAuthentication.RequireHttpsMetadata;
                });

            services.AddSwaggerGen(setupAction =>
            {
                setupAction.SwaggerDoc(
                    $"ClassifiedAds",
                    new OpenApiInfo()
                    {
                        Title = "ClassifiedAds API",
                        Version = "1",
                        Description = "ClassifiedAds API Specification.",
                        Contact = new OpenApiContact
                        {
                            Email = "abc.xyz@gmail.com",
                            Name = "Phong Nguyen",
                            Url = new Uri("https://github.com/phongnguyend"),
                        },
                        License = new OpenApiLicense
                        {
                            Name = "MIT License",
                            Url = new Uri("https://opensource.org/licenses/MIT"),
                        },
                    });

                setupAction.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "Input your Bearer token to access this API",
                });

                setupAction.AddSecurityDefinition("Oidc", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri(AppSettings.IdentityServerAuthentication.Authority + "/connect/token", UriKind.Absolute),
                            AuthorizationUrl = new Uri(AppSettings.IdentityServerAuthentication.Authority + "/connect/authorize", UriKind.Absolute),
                            Scopes = new Dictionary<string, string>
                            {
                                { "openid", "OpenId" },
                                { "profile", "Profile" },
                                { "ClassifiedAds.WebAPI", "ClassifiedAds WebAPI" },
                            },
                        },
                        ClientCredentials = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri(AppSettings.IdentityServerAuthentication.Authority + "/connect/token", UriKind.Absolute),
                            Scopes = new Dictionary<string, string>
                            {
                                { "ClassifiedAds.WebAPI", "ClassifiedAds WebAPI" },
                            },
                        },
                    },
                });

                setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Oidc",
                            },
                        }, new List<string>()
                    },
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                        }, new List<string>()
                    },
                });
            });

            services.AddMonitoringServices(AppSettings.Monitoring);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ICurrentUser, CurrentWebUser>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDebuggingMiddleware();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCors(AppSettings.CORS.AllowAnyOrigin ? "AllowAnyOrigin" : "AllowedOrigins");

            app.UseSwagger();

            app.UseSwaggerUI(setupAction =>
            {
                setupAction.OAuthClientId("Swagger");
                setupAction.OAuthClientSecret("secret");
                setupAction.OAuthUsePkce();

                setupAction.SwaggerEndpoint(
                    "/swagger/ClassifiedAds/swagger.json",
                    "ClassifiedAds API");

                setupAction.RoutePrefix = string.Empty;

                setupAction.DefaultModelExpandDepth(2);
                setupAction.DefaultModelRendering(ModelRendering.Model);
                setupAction.DocExpansion(DocExpansion.None);
                setupAction.EnableDeepLinking();
                setupAction.DisplayOperationId();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMonitoringServices(AppSettings.Monitoring);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotificationHub>("/hubs/notification").RequireCors("SignalRHubs");
            });
        }
    }
}
