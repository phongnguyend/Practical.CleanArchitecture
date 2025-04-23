using ClassifiedAds.EndToEndTests.Configuration;
using Microsoft.Playwright;
using System.Threading.Tasks;

namespace ClassifiedAds.EndToEndTests.Pages;

public class LoginPage
{
    private readonly IPage _page;
    private readonly AppSettings _appSettings;

    public LoginPage(IPage page, AppSettings appSettings)
    {
        _page = page;
        _appSettings = appSettings;
    }

    public async Task GoToAsync()
    {
        var loginUrl = _appSettings.Login.Url;

        await _page.GotoAsync(loginUrl);
    }

    public async Task SetUserNameAsync(string userName)
    {
        await _page.FillAsync("#Username", userName);
    }

    public async Task SetPasswordAsync(string password)
    {
        await _page.FillAsync("#Password", password);
    }

    public async Task SetRememberMyLoginAsync(bool rememberMyLogin)
    {
        bool isChecked = await _page.IsCheckedAsync("#RememberLogin");
        var btn = _page.Locator("xpath=//input[@id='RememberLogin']/following-sibling::div");
        if (rememberMyLogin)
        {
            if (!isChecked)
            {
                await btn.ClickAsync();
            }
        }
        else
        {
            if (isChecked)
            {
                await btn.ClickAsync();
            }
        }
    }

    internal async Task<ConsentPage> Login()
    {
        await _page.ClickAsync("#btnLogin");

        return new ConsentPage(_page);
    }
}
