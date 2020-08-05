using ClassifiedAds.IntegrationTests.Configuration;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ClassifiedAds.IntegrationTests
{
    public class TestBase
    {
        public AppSettings AppSettings { get; set; } = new AppSettings();

        public TestBase()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

            var configuration = builder.Build();

            configuration.Bind(AppSettings);
        }
    }
}
