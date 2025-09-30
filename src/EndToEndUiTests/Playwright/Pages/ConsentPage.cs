using Microsoft.Playwright;
using System.Threading.Tasks;

namespace ClassifiedAds.EndToEndTests.Pages;

public class ConsentPage
{
    private readonly IPage _page;

    public ConsentPage(IPage page)
    {
        _page = page;
    }

    public async Task SetRememberMyDecisionAsync(bool rememberMyDecision)
    {
        var remembered = await _page.IsCheckedAsync("#RememberConsent");
        var btn = _page.Locator("xpath=//input[@id='RememberConsent']/following-sibling::div");
        if (rememberMyDecision)
        {
            if (!remembered)
            {
                await btn.ClickAsync();
            }

        }
        else
        {
            if (remembered)
            {
                await btn.ClickAsync();
            }
        }
    }

    public async Task ConfirmAsync()
    {
       await _page.ClickAsync("#btnYes");
    }
}
