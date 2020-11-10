using ClassifiedAds.BlazorServerSide.ConfigurationOptions;
using ClassifiedAds.Infrastructure.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ClassifiedAds.BlazorServerSide
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseClassifiedAdsLogger(configuration =>
                     {
                         var appSettings = new AppSettings();
                         configuration.Bind(appSettings);
                         return appSettings.Logging;
                     });
                });
    }
}
