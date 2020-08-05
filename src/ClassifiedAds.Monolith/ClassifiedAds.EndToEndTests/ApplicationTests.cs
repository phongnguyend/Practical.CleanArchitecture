using ClassifiedAds.EndToEndTests.Navigation;
using System.Threading;
using Xunit;

namespace ClassifiedAds.EndToEndTests
{
    public class ApplicationTests : TestBase
    {
        [Fact]
        public void HomePage()
        {
            var navigator = new Navigator(Driver, AppSettings);

            var loginPage = navigator.LoginPage();
            loginPage.UserName = AppSettings.Login.UserName;
            loginPage.Password = AppSettings.Login.Password;
            loginPage.RememberMyLogin = true;
            Thread.Sleep(2000);

            var consentPage = loginPage.Login();
            Thread.Sleep(2000);
            consentPage.RememberMyDecision = false;
            Thread.Sleep(2000);
            consentPage.Confirm();
            Thread.Sleep(2000);

            navigator.HomePage();
            Thread.Sleep(2000);

            navigator.PrivacyPage();
            Thread.Sleep(2000);

            navigator.AuthorizedActionPage();
            Thread.Sleep(2000);

            navigator.Logout();
            Thread.Sleep(2000);
        }

    }
}
