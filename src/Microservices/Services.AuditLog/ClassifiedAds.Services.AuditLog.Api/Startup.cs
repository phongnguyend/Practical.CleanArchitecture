using ClassifiedAds.Infrastructure.Monitoring;
using ClassifiedAds.Infrastructure.Web.Filters;
using ClassifiedAds.Services.AuditLog.ConfigurationOptions;
using ClassifiedAds.Services.AuditLog.RateLimiterPolicies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Polly;
using System;
using System.Reflection;

namespace ClassifiedAds.Services.AuditLog;

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
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        AppSettings.ConnectionStrings.MigrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

        services.AddMonitoringServices(AppSettings.Monitoring);

        services.AddControllers(configure =>
        {
            configure.Filters.Add(typeof(GlobalExceptionFilter));
        })
        .ConfigureApiBehaviorOptions(options =>
        {
        })
        .AddJsonOptions(options =>
        {
        })
        .AddDapr();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAnyOrigin", builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
        });

        services.AddDateTimeProvider();
        services.AddApplicationServices();

        services.AddAuditLogModule(AppSettings);
        services.AddHostedServicesAuditLogModule();

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = AppSettings.IdentityServerAuthentication.Provider switch
            {
                "OpenIddict" => "OpenIddict",
                _ => JwtBearerDefaults.AuthenticationScheme
            };
        })
        .AddJwtBearer(options =>
        {
            options.Authority = AppSettings.IdentityServerAuthentication.Authority;
            options.Audience = AppSettings.IdentityServerAuthentication.ApiName;
            options.RequireHttpsMetadata = AppSettings.IdentityServerAuthentication.RequireHttpsMetadata;
        })
        .AddJwtBearer("OpenIddict", options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidIssuer = AppSettings.IdentityServerAuthentication.OpenIddict.IssuerUri,
                TokenDecryptionKey = new X509SecurityKey(AppSettings.IdentityServerAuthentication.OpenIddict.TokenDecryptionCertificate.FindCertificate()),
                IssuerSigningKey = new X509SecurityKey(AppSettings.IdentityServerAuthentication.OpenIddict.IssuerSigningCertificate.FindCertificate()),
            };
        });

        services.AddRateLimiter(options =>
        {
            options.AddPolicy<string, GetAuditLogsRateLimiterPolicy>(RateLimiterPolicyNames.GetAuditLogsPolicy);
        });

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        Policy.Handle<Exception>().WaitAndRetry(new[]
        {
            TimeSpan.FromSeconds(10),
            TimeSpan.FromSeconds(20),
            TimeSpan.FromSeconds(30),
        })
        .Execute(() =>
        {
            app.MigrateAuditLogDb();
        });

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseCloudEvents();

        app.UseCors("AllowAnyOrigin");

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseRateLimiter();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapSubscribeHandler();
            endpoints.MapControllers();
        });
    }
}
