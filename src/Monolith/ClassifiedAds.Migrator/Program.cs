using ClassifiedAds.Infrastructure.HealthChecks;
using ClassifiedAds.Infrastructure.Logging;
using ClassifiedAds.Persistence;
using DbUp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using System;
using System.Reflection;

var builder = Host.CreateDefaultBuilder(args)
.UseClassifiedAdsLogger(configuration =>
{
    return new LoggingOptions();
})
.ConfigureServices((hostContext, services) =>
{
    var configuration = hostContext.Configuration;

    if (string.Equals(configuration["CheckDependency:Enabled"], "true", StringComparison.OrdinalIgnoreCase))
    {
        NetworkPortCheck.Wait(configuration["CheckDependency:Host"], 5);
    }

    services.AddDateTimeProvider();

    services.AddDbContext<AdsDbContext>(options => options.UseSqlServer(configuration["ConnectionStrings:ClassifiedAds"], sql =>
    {
        sql.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
    }));
});

var app = builder.Build();

Policy.Handle<Exception>().WaitAndRetry(
[
    TimeSpan.FromSeconds(10),
    TimeSpan.FromSeconds(22),
    TimeSpan.FromSeconds(30),
])
.Execute(() =>
{
    app.MigrateAdsDb();

    var upgrader = DeployChanges.To
    .SqlDatabase(app.Services.GetRequiredService<IConfiguration>().GetConnectionString("ClassifiedAds"))
    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
    .LogToConsole()
    .Build();

    var result = upgrader.PerformUpgrade();

    if (!result.Successful)
    {
        throw result.Error;
    }
});
