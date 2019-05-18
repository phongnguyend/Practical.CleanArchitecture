using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
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
