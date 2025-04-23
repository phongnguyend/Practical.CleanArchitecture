using ClassifiedAds.EndToEndTests.Configuration;
using ClassifiedAds.EndToEndTests.Pages;
using Microsoft.Playwright;
using System.Threading.Tasks;

namespace ClassifiedAds.EndToEndTests.Navigation;

public class Navigator
{
    private readonly IPage _page;
    private readonly AppSettings _appSettings;

    public Navigator(IPage page, AppSettings appSettings)
    {
        _page = page;
        _appSettings = appSettings;
    }

    public async Task HomePageAsync()
    {
        await _page.ClickAsync("body > header > nav > div > div > ul > li:nth-child(1) > a");
    }

    public async Task PrivacyPageAsync()
    {
        await _page.ClickAsync("body > header > nav > div > div > ul > li:nth-child(2) > a");
    }

    public async Task AuthorizedActionPageAsync()
    {
        await _page.ClickAsync("body > header > nav > div > div > ul > li:nth-child(3) > a");
    }

    public async Task<LoginPage> LoginPageAsync()
    {
        var loginPage = new LoginPage(_page, _appSettings);
        await loginPage.GoToAsync();
        return loginPage;
    }

    public async Task LogoutAsync()
    {
        await _page.ClickAsync("body > header > nav > div > div > ul > li:nth-child(5) > a");
    }
}
