using OpenQA.Selenium;

namespace ClassifiedAds.FunctionalTests.Pages
{
    public class LoginPage
    {
        IWebDriver _driver;

        public LoginPage(IWebDriver driver)
        {
            _driver = driver;
        }

        public void GoTo()
        {
            var loginUrl = Configuration.Login_Url;
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
