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

    if (bool.TryParse(configuration["CheckDependency:Enabled"], out var enabled) && enabled)
    {
        NetworkPortCheck.Wait(configuration["CheckDependency:Host"], 5);
    }

    services.AddDateTimeProvider();

    services.AddDbContext<AdsDbContext>(options => options.UseSqlServer(configuration["ConnectionStrings:ClassifiedAds"], sql =>
    {
        sql.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);

        if (int.TryParse(configuration["CommandTimeout"], out var commandTimeout))
        {
            sql.CommandTimeout(commandTimeout);
        }
    }));
});

var app = builder.Build();
var configuration = app.Services.GetRequiredService<IConfiguration>();

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
    .SqlDatabase(configuration.GetConnectionString("ClassifiedAds"))
    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
    .LogToConsole()
    .Build();

    var result = upgrader.PerformUpgrade();

    if (!result.Successful)
    {
        throw result.Error;
    }
});
