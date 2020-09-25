using ClassifiedAds.EndToEndTests.Configuration;
using ClassifiedAds.EndToEndTests.Pages;
using OpenQA.Selenium;

namespace ClassifiedAds.EndToEndTests.Navigation
{
    public class Navigator
    {
        private readonly IWebDriver _driver;
        private readonly AppSettings _appSettings;
        private readonly IJavaScriptExecutor _javaScriptExecutor;

        public Navigator(IWebDriver driver, AppSettings appSettings)
        {
            _driver = driver;
            _appSettings = appSettings;
            _javaScriptExecutor = (IJavaScriptExecutor)_driver;
        }

        public void HomePage()
        {
            var link = _driver.FindElement(By.CssSelector("body > header > nav > div > div > ul > li:nth-child(1) > a"));
            _javaScriptExecutor.ExecuteScript("arguments[0].click();", link);
        }

        public void PrivacyPage()
        {
            var link = _driver.FindElement(By.CssSelector("body > header > nav > div > div > ul > li:nth-child(2) > a"));
            _javaScriptExecutor.ExecuteScript("arguments[0].click();", link);
        }

        public void AuthorizedActionPage()
        {
            var link = _driver.FindElement(By.CssSelector("body > header > nav > div > div > ul > li:nth-child(3) > a"));
            _javaScriptExecutor.ExecuteScript("arguments[0].click();", link);
        }

        public LoginPage LoginPage()
        {
            var loginPage = new LoginPage(_driver, _appSettings);
            loginPage.GoTo();
            return loginPage;
        }

        public void Logout()
        {
            var logoutLink = _driver.FindElement(By.CssSelector("body > header > nav > div > div > ul > li:nth-child(5) > a"));
            _javaScriptExecutor.ExecuteScript("arguments[0].click();", logoutLink);
        }
    }
}
