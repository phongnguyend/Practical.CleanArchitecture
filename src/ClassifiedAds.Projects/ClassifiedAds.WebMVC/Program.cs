using ClassifiedAds.Infrastructure.Configuration;
using ClassifiedAds.Infrastructure.HealthChecks;
using ClassifiedAds.Persistence;
using ClassifiedAds.WebMVC.ConfigurationOptions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using Serilog;

namespace ClassifiedAds.WebMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((ctx, builder) =>
                {
                    var config = builder.Build();
                    var appSettings = new AppSettings();
                    config.Bind(appSettings);

                    builder.AddEFConfiguration(() =>
                    {
                        if (appSettings.CheckDependency.Enabled)
                        {
                            NetworkPortCheck.Wait(appSettings.CheckDependency.Host, 5);
                        }

                        var dbContextOptionsBuilder = new DbContextOptionsBuilder<AdsDbContext>();
                        dbContextOptionsBuilder.UseSqlServer(appSettings.ConnectionStrings.ClassifiedAds);
                        return new AdsDbContext(dbContextOptionsBuilder.Options);
                    });

                    //builder.AddSqlConfigurationVariables(appSettings.ConnectionStrings.ClassifiedAds);

                    if (ctx.HostingEnvironment.IsDevelopment())
                    {
                        return;
                    }

                    builder.AddAzureKeyVault($"https://{appSettings.KeyVaultName}.vault.azure.net/");
                })
                .ConfigureLogging(logging =>
                {
                    //logging.AddEventLog(new EventLogSettings
                    //{
                    //    LogName = "ClassifiedAds",
                    //    SourceName = "WebMVC",
                    //    Filter = (a, b) => b >= LogLevel.Information
                    //});
                })
                .UseSerilog();
    }
}
