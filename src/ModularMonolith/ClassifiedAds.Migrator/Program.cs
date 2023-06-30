using ClassifiedAds.Contracts.Identity.Services;
using ClassifiedAds.Infrastructure.HealthChecks;
using ClassifiedAds.Infrastructure.Logging;
using ClassifiedAds.Modules.Identity.Repositories;
using ClassifiedAds.Modules.Identity.Services;
using DbUp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using System;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
var configuration = builder.Configuration;

builder.WebHost.UseClassifiedAdsLogger(configuration =>
{
    return new LoggingOptions();
});

if (string.Equals(configuration["CheckDependency:Enabled"], "true", StringComparison.OrdinalIgnoreCase))
{
    NetworkPortCheck.Wait(configuration["CheckDependency:Host"], 5);
}

services.AddDateTimeProvider();

services.AddAuditLogModule(opt =>
{
    configuration.GetSection("Modules:AuditLog").Bind(opt);
    opt.ConnectionStrings.MigrationsAssembly = Assembly.GetExecutingAssembly().GetName().Name;
})
.AddConfigurationModule(opt =>
{
    configuration.GetSection("Modules:Configuration").Bind(opt);
    opt.ConnectionStrings.MigrationsAssembly = Assembly.GetExecutingAssembly().GetName().Name;
})
.AddIdentityModuleCore(opt =>
{
    configuration.GetSection("Modules:Identity").Bind(opt);
    opt.ConnectionStrings.MigrationsAssembly = Assembly.GetExecutingAssembly().GetName().Name;
})
.AddNotificationModule(opt =>
{
    configuration.GetSection("Modules:Notification").Bind(opt);
    opt.ConnectionStrings.MigrationsAssembly = Assembly.GetExecutingAssembly().GetName().Name;
})
.AddProductModule(opt =>
{
    configuration.GetSection("Modules:Product").Bind(opt);
    opt.ConnectionStrings.MigrationsAssembly = Assembly.GetExecutingAssembly().GetName().Name;
})
.AddStorageModule(opt =>
{
    configuration.GetSection("Modules:Storage").Bind(opt);
    opt.ConnectionStrings.MigrationsAssembly = Assembly.GetExecutingAssembly().GetName().Name;
})
.AddApplicationServices();

services.AddDataProtection()
    .PersistKeysToDbContext<IdentityDbContext>()
    .SetApplicationName("ClassifiedAds");

services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
services.AddScoped<ICurrentUser, CurrentWebUser>();

// Configure the HTTP request pipeline.
var app = builder.Build();

Policy.Handle<Exception>().WaitAndRetry(new[]
{
            TimeSpan.FromSeconds(10),
            TimeSpan.FromSeconds(20),
            TimeSpan.FromSeconds(30),
})
.Execute(() =>
{
    app.MigrateAuditLogDb();
    app.MigrateConfigurationDb();
    app.MigrateIdentityDb();
    app.MigrateNotificationDb();
    app.MigrateProductDb();
    app.MigrateStorageDb();

    var upgrader = DeployChanges.To
    .SqlDatabase(configuration["Modules:AuditLog:ConnectionStrings:Default"])
    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
    .LogToConsole()
    .Build();

    var result = upgrader.PerformUpgrade();

    if (!result.Successful)
    {
        throw result.Error;
    }
});

app.Run();