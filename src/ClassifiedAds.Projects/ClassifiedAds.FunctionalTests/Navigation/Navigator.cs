using ClassifiedAds.FunctionalTests.Pages;
using OpenQA.Selenium;

namespace ClassifiedAds.FunctionalTests.Navigation
{
    public class Navigator
    {
        IWebDriver _driver;
        IJavaScriptExecutor _javaScriptExecutor;

        public Navigator(IWebDriver driver)
        {
            _driver = driver;
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
            var loginPage = new LoginPage(_driver);
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
