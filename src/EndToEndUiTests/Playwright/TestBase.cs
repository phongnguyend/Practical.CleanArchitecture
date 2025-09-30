using ClassifiedAds.EndToEndTests.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace ClassifiedAds.EndToEndTests;

public class TestBase : IDisposable
{
    public AppSettings AppSettings { get; set; } = new AppSettings();

    public TestBase()
    {
        Microsoft.Playwright.Program.Main(["install"]);

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

        var configuration = builder.Build();

        configuration.Bind(AppSettings);
    }

    public void Dispose()
    {
    }
}
