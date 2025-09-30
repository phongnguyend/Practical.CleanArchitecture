using ClassifiedAds.EndToEndTests.Navigation;
using Microsoft.Playwright;
using System.Threading.Tasks;
using Xunit;

namespace ClassifiedAds.EndToEndTests;

public class ApplicationTests : TestBase
{
    [Fact]
    public async Task HomePage()
    {
        using var playwright = await Playwright.CreateAsync();

        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
        });

        var page = await browser.NewPageAsync();

        var navigator = new Navigator(page, AppSettings);

        var loginPage = await navigator.LoginPageAsync();
        await loginPage.SetUserNameAsync(AppSettings.Login.UserName);
        await loginPage.SetPasswordAsync(AppSettings.Login.Password);
        await loginPage.SetRememberMyLoginAsync(true);
        await Task.Delay(2000);

        var consentPage = await loginPage.Login();
        await Task.Delay(2000);

        if (AppSettings.Login.Consent)
        {
            await consentPage.SetRememberMyDecisionAsync(false);
            await Task.Delay(2000);
            await consentPage.ConfirmAsync();
            await Task.Delay(2000);
        }

        await navigator.HomePageAsync();
        await Task.Delay(2000);

        await navigator.PrivacyPageAsync();
        await Task.Delay(2000);

        await navigator.AuthorizedActionPageAsync();
        await Task.Delay(2000);

        await navigator.LogoutAsync();
        await Task.Delay(2000);
    }

}
