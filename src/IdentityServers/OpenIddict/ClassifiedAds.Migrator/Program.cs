using ClassifiedAds.Infrastructure.HealthChecks;
using ClassifiedAds.Infrastructure.Logging;
using ClassifiedAds.Persistence;
using DbUp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using SqlServerQueryHelper.EntityFrameworkCore;
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

    services.AddDbContext<OpenIddictDbContext>(options =>
    {
        options.UseSqlServer(configuration["ConnectionStrings:IdentityServer"], sql =>
        {
            sql.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);

            if (int.TryParse(configuration["CommandTimeout"], out var commandTimeout))
            {
                sql.CommandTimeout(commandTimeout);
            }
        });

        // Register the entity sets needed by OpenIddict.
        options.UseOpenIddict();
    });
});

var app = builder.Build();
var configuration = app.Services.GetRequiredService<IConfiguration>();

Policy.Handle<Exception>().WaitAndRetry(
[
    TimeSpan.FromSeconds(10),
    TimeSpan.FromSeconds(20),
    TimeSpan.FromSeconds(30),
])
.Execute(() =>
{
    app.MigrateOpenIddictDb();

    var upgrader = DeployChanges.To
    .SqlDatabase(configuration.GetConnectionString("IdentityServer"))
    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
    .LogToConsole()
    .Build();

    var result = upgrader.PerformUpgrade();

    if (!result.Successful)
    {
        throw result.Error;
    }
});

using var serviceScope = app.Services.CreateScope();
var context = serviceScope.ServiceProvider.GetRequiredService<OpenIddictDbContext>();
context.Database.ExecuteSqlFiles("Scripts/Types", log: Console.WriteLine);
context.Database.ExecuteSqlFiles("Scripts/Views", log: Console.WriteLine);
context.Database.ExecuteSqlFiles("Scripts/Functions", log: Console.WriteLine);
context.Database.ExecuteSqlFiles("Scripts/Stored Procedures", log: Console.WriteLine);
context.Database.ExecuteSqlFiles("Scripts/Indexes", log: Console.WriteLine);