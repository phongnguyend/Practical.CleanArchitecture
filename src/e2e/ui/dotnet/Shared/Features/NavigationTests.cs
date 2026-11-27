using Xunit;

namespace ClassifiedAds.EndToEndTests.Features;

public sealed class NavigationTests : TestBase
{
    [UiTheory]
    [Apps("Products|/products", "Files|/files", "Users|/users", "Roles|/roles", "Settings|/settings", "Audit Logs|/auditlogs")]
    public Task NavigateAndReload(string app, string section, string path) => Run(app, async s =>
    {
        await s.Open(section);
        await s.Path(path);
        await s.Browser.Reload();
        await s.Browser.Wait(async () => (await s.Browser.Evaluate($"{UiBrowser.Element(Selectors.Action(section, Selectors.Nav))}?.classList.contains('active')"))?.GetValue<bool>() == true, "Active navigation tab missing");
        await s.Open("Home");
        await s.Browser.Visible(Selectors.Text("Phong Nguyen"));
    });

    [UiTheory]
    [Apps]
    public Task HomeProjectLink(string app) => Run(app, async s =>
    {
        await s.Open("Home");
        await s.Browser.Attribute("//a[contains(normalize-space(.),'Practical.CleanArchitecture')]", "href", "https://github.com/phongnguyend/Practical.CleanArchitecture");
    });

    [UiTheory]
    [Apps("Light", "Dark")]
    public Task ThemePersists(string app, string theme) => Run(app, async s =>
    {
        await s.Theme(theme);
        await s.Browser.Reload();
        await s.Browser.Attribute("//html", "data-bs-theme", theme.ToLowerInvariant());
    });

    [UiTheory]
    [Apps]
    public Task SystemTheme(string app) => Run(app, async s =>
    {
        await s.Theme("System");
        foreach (var scheme in new[] { "dark", "light" })
        {
            await s.Browser.Media(scheme);
            await s.Browser.Attribute("//html", "data-bs-theme", scheme);
        }
    });

    [HubTheory]
    [HubApps]
    public Task NotificationToast(string app) => Run(app, async s =>
    {
        await s.Browser.Go(Settings.Url(app));
        await s.Browser.Visible(Selectors.Text("Connected to NotificationHub"));
    });
}
