using ClassifiedAds.Infrastructure.HealthChecks;
using ClassifiedAds.Infrastructure.Logging;
using ClassifiedAds.Persistence;
using DbUp;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using System;
using System.Reflection;

namespace ClassifiedAds.Migrator;

public class Program
{
    public static void Main(string[] args)
    {
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

        services.AddPersistence(configuration["ConnectionStrings:ClassifiedAds"],
            Assembly.GetExecutingAssembly().GetName().Name);

        services.AddDbContext<OpenIddictDbContext>(options =>
        {
            options.UseSqlServer(configuration["ConnectionStrings:IdentityServer"], sql =>
            {
                sql.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
            });

            // Register the entity sets needed by OpenIddict.
            options.UseOpenIddict();
        });

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
            app.MigrateOpenIddictDb();

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

        app.Run();
    }
}
