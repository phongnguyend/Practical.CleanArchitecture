using OpenQA.Selenium;

namespace ClassifiedAds.EndToEndTests.Pages
{
    public class ConsentPage
    {
        IWebDriver _driver;

        public ConsentPage(IWebDriver driver)
        {
            _driver = driver;
        }

        public bool RememberMyDecision
        {
            set
            {
                var remember = _driver.FindElement(By.Id("RememberConsent"));
                var btn = _driver.FindElement(By.XPath("//input[@id='RememberConsent']/following-sibling::div"));
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

        public void Confirm()
        {
            var loginButton = _driver.FindElement(By.Id("btnYes"));
            loginButton.Click();
        }
    }
}
