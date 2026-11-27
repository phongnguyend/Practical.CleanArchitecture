using Xunit;

namespace ClassifiedAds.EndToEndTests.Features;

public sealed class WorkspaceLayoutTests : TestBase
{
    [UiTheory]
    [Apps("1440", "390")]
    public Task ResponsiveThemesDialogsAndTabs(string app, string width) => Run(app, async s =>
    {
        await s.Browser.Resize(int.Parse(width), 900);
        await s.Open("Home");
        await s.Browser.Evaluate("document.querySelector('.skip-link').focus()");
        await s.Browser.Key("Enter");
        await s.Browser.Focused("//*[@id='main-content']");
        foreach (var theme in new[] { "Light", "Dark" })
        {
            await s.Theme(theme);
            await s.Open("Home");
            await s.Open("Products");
            await s.Browser.Visible(Selectors.Action("Admin", Selectors.Nav));
            await Checks.NoOverflow(s);
            await s.Browser.Screenshot(Path.Combine(s.Directory, "products-" + theme + "-" + width + ".png"));
            await s.Click("Import Csv");
            await s.Browser.Visible(Selectors.Dialog);
            foreach (var action in new[] { "Import", "Cancel" })
            {
                await s.Browser.Wait(async () => (await s.Browser.Evaluate($$"""
                    (() => {
                        const rect = {{UiBrowser.Element(Selectors.Action(action, Selectors.Dialog))}}.getBoundingClientRect();
                        return rect.top >= 0 && rect.left >= 0 && rect.bottom <= innerHeight && rect.right <= innerWidth;
                    })()
                    """))!.GetValue<bool>(), "Dialog action outside viewport: " + action);
            }

            await s.Browser.Screenshot(Path.Combine(s.Directory, "import-" + theme + "-" + width + ".png"));
            await s.Browser.Key("Escape");
            await s.Browser.Hidden(Selectors.Dialog);
            await s.Playground("Email");
            var primary = Selectors.Action("Admin", Selectors.Nav);
            foreach (var tab in new[] { primary, Selectors.Action("Playgrounds", "//nav[@aria-label='Admin sections']"), Selectors.Action("Email", "//nav[@aria-label='Playground channels']") })
            {
                await s.Browser.Wait(async () => (await s.Browser.Evaluate($"getComputedStyle({UiBrowser.Element(tab)}).backgroundColor === getComputedStyle({UiBrowser.Element(primary)}).backgroundColor"))!.GetValue<bool>(), "Tab colors differ");
                Assert.Equal("8px", (await s.Browser.Evaluate($"getComputedStyle({UiBrowser.Element(tab)}).borderRadius"))!.GetValue<string>());
            }
        }
    });

    [UiTheory]
    [Apps("1440", "390")]
    public Task HeaderActionsAlignRight(string app, string width) => Run(app, async s =>
    {
        await s.Browser.Resize(int.Parse(width), 900);
        foreach (var (section, action) in new[] { ("Products", "Add Product"), ("Files", "Upload File"), ("Users", "Add User"), ("Roles", "Add Role"), ("Settings", "Add") })
        {
            await s.Open(section);
            await s.Browser.Visible(Selectors.Action(action));
            var gap = await s.Browser.Evaluate($$"""
                (() => {
                    const element = {{UiBrowser.Element(Selectors.Action(action))}};
                    const panel = element.closest('.card-header');
                    const style = getComputedStyle(panel);
                    return panel.getBoundingClientRect().right - element.getBoundingClientRect().right
                        - parseFloat(style.paddingRight) - parseFloat(style.borderRightWidth);
                })()
                """);
            Assert.InRange(Math.Abs(gap!.GetValue<double>()), 0, 2);
        }
    });
}
