using ClassifiedAds.DomainServices.Identity;
using ClassifiedAds.WebMVC.Authorization;
using ClassifiedAds.WebMVC.ClaimsTransformations;
using ClassifiedAds.WebMVC.ConfigurationOptions;
using ClassifiedAds.WebMVC.Filters;
using ClassifiedAds.WebMVC.HttpHandlers;
using ClassifiedAds.WebMVC.Identity;
using ClassifiedAds.WebMVC.Middleware;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.IO;

namespace ClassifiedAds.WebMVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;

            Directory.CreateDirectory(Path.Combine(env.ContentRootPath, "logs"));
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(Path.Combine(env.ContentRootPath, "logs", "log.txt"),
                    fileSizeLimitBytes: 10 * 1024 * 1024,
                    rollOnFileSizeLimit: true,
                    shared: true,
                    flushToDiskInterval: TimeSpan.FromSeconds(1))
                .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IValidateOptions<AppSettings>, AppSettingsValidation>());

            var appSettings = new AppSettings();
            Configuration.Bind(appSettings);
            var validationResult = appSettings.Validate();
            if (validationResult.Failed)
            {
                throw new Exception(validationResult.FailureMessage);
            }

            services.Configure<AppSettings>(Configuration);

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddControllersWithViews(setupAction =>
            {
                setupAction.Filters.Add(typeof(CustomActionFilter));
                setupAction.Filters.Add(typeof(CustomResultFilter));
                setupAction.Filters.Add(typeof(CustomAuthorizationFilter));
                setupAction.Filters.Add(typeof(CustomExceptionFilter));
            })
            .AddNewtonsoftJson();

            services.AddPersistence(appSettings.ConnectionStrings.ClassifiedAds)
                    .AddDomainServices()
                    .AddMessageHandlers();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies", options =>
            {
                options.AccessDeniedPath = "/Authorization/AccessDenied";
            })
            .AddOpenIdConnect("oidc", options =>
            {
                options.SignInScheme = "Cookies";
                options.Authority = appSettings.OpenIdConnect.Authority;
                options.ClientId = appSettings.OpenIdConnect.ClientId;
                options.ResponseType = "code id_token";
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("ClassifiedAds.WebAPI");
                options.Scope.Add("offline_access");
                options.SaveTokens = true;
                options.ClientSecret = "secret";
                options.GetClaimsFromUserInfoEndpoint = true;
                options.RequireHttpsMetadata = appSettings.OpenIdConnect.RequireHttpsMetadata;
            });
            services.AddSingleton<IClaimsTransformation, CustomClaimsTransformation>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("CustomPolicy", policy =>
                {
                    policy.AddRequirements(new CustomRequirement());
                });
            })
            .AddSingleton<IAuthorizationHandler, CustomRequirementHandler>();


            services.AddTransient<ProfilingHttpHandler>();
            services.AddHttpClient("")
                    .AddHttpMessageHandler<ProfilingHttpHandler>();

            services.AddMiniProfiler(options =>
            {
                options.RouteBasePath = "/profiler";// access /profiler/results to see last profile check
                options.PopupRenderPosition = StackExchange.Profiling.RenderPosition.BottomLeft;
                options.PopupShowTimeWithChildren = true;
            })
            .AddEntityFramework();

            services.AddHealthChecks()
                .AddSqlServer(connectionString: appSettings.ConnectionStrings.ClassifiedAds,
                    healthQuery: "SELECT 1;",
                    name: "Sql Server",
                    failureStatus: HealthStatus.Degraded)
                .AddUrlGroup(new Uri(appSettings.OpenIdConnect.Authority),
                    name: "Identity Server",
                    failureStatus: HealthStatus.Degraded)
                .AddUrlGroup(new Uri(appSettings.ResourceServer.Endpoint),
                    name: "Resource (Web API) Server",
                    failureStatus: HealthStatus.Degraded);

            services.AddHealthChecksUI(setupSettings: setup =>
            {
                setup.AddHealthCheckEndpoint("Basic Health Check", $"{appSettings.CurrentUrl}/healthcheck");
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ICurrentUser, CurrentUser>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.MigrateDb();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSecurityHeaders();

            app.UseIPFiltering();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMiniProfiler();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHealthChecks("/healthcheck", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
                    [HealthStatus.Unhealthy] =StatusCodes.Status503ServiceUnavailable
                }
            });
            app.UseHealthChecksUI(); // /healthchecks-ui#/healthchecks

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
