using ClassifiedAds.EndToEndTests.Configuration;
using OpenQA.Selenium;

namespace ClassifiedAds.EndToEndTests.Pages
{
    public class LoginPage
    {
        private readonly IWebDriver _driver;
        private readonly AppSettings _appSettings;

        public LoginPage(IWebDriver driver, AppSettings appSettings)
        {
            _driver = driver;
            _appSettings = appSettings;
        }

        public void GoTo()
        {
            var loginUrl = _appSettings.Login.Url;
            _driver.Navigate().GoToUrl(loginUrl);
        }

        public string UserName
        {
            set
            {
                var emailInput = _driver.FindElement(By.Id("Username"));
                emailInput.SendKeys(value);
            }
        }

        public string Password
        {
            set
            {
                var emailInput = _driver.FindElement(By.Id("Password"));
                emailInput.SendKeys(value);
            }
        }

        public bool RememberMyLogin
        {
            set
            {
                var remember = _driver.FindElement(By.Id("RememberLogin"));
                var btn = _driver.FindElement(By.XPath("//input[@id='RememberLogin']/following-sibling::div"));
                if (value)
                {
                    if (!remember.Selected)
                    {
                        btn.Click();
                    }

                }
                else
                {
                    if (remember.Selected)
                    {
                        btn.Click();
                    }
                }
            }
        }

        internal ConsentPage Login()
        {
            var loginButton = _driver.FindElement(By.Id("btnLogin"));
            loginButton.Click();

            return new ConsentPage(_driver);
        }
    }
}
