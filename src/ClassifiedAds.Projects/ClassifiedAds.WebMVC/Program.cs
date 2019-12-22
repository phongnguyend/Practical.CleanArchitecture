﻿using ClassifiedAds.WebMVC.ConfigurationProviders;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
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

                    builder.AddEFConfiguration(o =>
                    {
                        o.UseSqlServer(config.GetConnectionString("ClassifiedAds"));
                    });

                    if (ctx.HostingEnvironment.IsDevelopment())
                    {
                        return;
                    }

                    builder.AddAzureKeyVault($"https://{config["KeyVaultName"]}.vault.azure.net/");
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
