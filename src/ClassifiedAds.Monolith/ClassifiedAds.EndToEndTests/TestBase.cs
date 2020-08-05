using ClassifiedAds.EndToEndTests.Configuration;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;

namespace ClassifiedAds.EndToEndTests
{
    public class TestBase : IDisposable
    {
        public IWebDriver Driver { get; private set; }

        public AppSettings AppSettings { get; set; } = new AppSettings();

        public TestBase()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();

            configuration.Bind(AppSettings);

            Driver = new ChromeDriver(AppSettings.ChromeDriverPath);
            Driver.Manage().Window.Maximize();
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
        }

        public void Dispose()
        {
            Driver.Dispose();
        }
    }
}
