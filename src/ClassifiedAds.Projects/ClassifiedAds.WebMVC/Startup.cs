using ClassifiedAds.WebMVC.Authorization;
using ClassifiedAds.WebMVC.ClaimsTransformations;
using ClassifiedAds.WebMVC.Filters;
using ClassifiedAds.WebMVC.HttpHandlers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.IO;

namespace ClassifiedAds.WebMVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc(setupAction =>
            {
                setupAction.Filters.Add(typeof(CustomActionFilter));
                setupAction.Filters.Add(typeof(CustomResultFilter));
                setupAction.Filters.Add(typeof(CustomAuthorizationFilter));
                setupAction.Filters.Add(typeof(CustomExceptionFilter));
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddPersistence(Configuration.GetConnectionString("ClassifiedAds"))
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
                options.Authority = Configuration["OpenIdConnect:Authority"];
                options.ClientId = Configuration["OpenIdConnect:ClientId"];
                options.ResponseType = "code id_token";
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("ClassifiedAds.WebAPI");
                options.Scope.Add("offline_access");
                options.SaveTokens = true;
                options.ClientSecret = "secret";
                options.GetClaimsFromUserInfoEndpoint = true;
                options.RequireHttpsMetadata = bool.Parse(Configuration["OpenIdConnect:RequireHttpsMetadata"]);
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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

            app.UseIPFiltering();

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMiniProfiler();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
