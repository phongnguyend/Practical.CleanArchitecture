using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassifiedAds.EndToEndTests
{
    public class TestBase : IDisposable
    {
        public IWebDriver Driver { get; private set; }

        public TestBase()
        {
            Driver = new ChromeDriver(Configuration.ChromeDriverPath);
            Driver.Manage().Window.Maximize();
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
        }

        public void Dispose()
        {
            Driver.Dispose();
        }
    }
}
