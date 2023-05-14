using ClassifiedAds.Infrastructure.Monitoring;
using ClassifiedAds.Services.Notification.ConfigurationOptions;
using ClassifiedAds.Services.Notification.Grpc.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace ClassifiedAds.Services.Notification.Grpc;

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
        services.AddGrpc();

        services.AddMonitoringServices(AppSettings.Monitoring);

        services.AddDateTimeProvider();
        services.AddApplicationServices();

        services.AddNotificationModule(AppSettings);

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

        services.AddAuthorization();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<EmailMessageService>();
            endpoints.MapGrpcService<SmsMessageService>();

            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
            });
        });
    }
}
