using Xunit;

namespace ClassifiedAds.EndToEndTests.Features;

public sealed class SettingsTests : TestBase
{
    [UiTheory]
    [Apps]
    public Task RequiredFieldAndKeyboardDismissal(string app) => Run(app, s => Checks.RequiredModal(s, "Settings", "Add", "Key"));

    [UiTheory]
    [Apps]
    public Task ExcelValidationAndCancel(string app) => Run(app, s => Checks.ImportValidation(s, "Settings", "Import Excel"));

    [UiTheory]
    [Apps]
    public Task ExcelDownload(string app) => Run(app, async s =>
    {
        await s.Open("Settings");
        Checks.Download(await s.Download("Export as Excel"), ".xlsx");
    });

    [UiTheory]
    [Apps]
    public Task ExcelImportResets(string app) => Run(app, async s =>
    {
        await s.Open("Settings");
        var file = await s.Download("Export as Excel");
        Checks.Download(file, ".xlsx");
        await Checks.ImportAndReset(s, "Import Excel", file);
    });

    [UiTheory("E2E_ALLOW_WRITES")]
    [Apps]
    public Task CrudMasksSensitiveValues(string app) => Run(app, async s =>
    {
        s.Own("configurationentries");
        var key = s.Prefix + "-setting";
        await s.Open("Settings");
        await s.Click("Add");
        await s.Fill("Key", key, Selectors.Dialog);
        await s.Fill("Value", "fixture-secret", Selectors.Dialog);
        await s.Fill("Description", "E2E configuration", Selectors.Dialog);
        await s.Browser.Check(Selectors.Field("Sensitive", Selectors.Dialog), true);
        await s.Response("configurationentries", "POST", () => s.Click("Save", Selectors.Dialog));
        var row = await s.Find("configurationentries", key);
        await s.Browser.Hidden(Selectors.Dialog);
        await s.Browser.Contains(Selectors.Row(key), "******");
        Assert.DoesNotContain("fixture-secret", await s.Browser.Text(Selectors.Row(key)));
        await s.Click("Edit", Selectors.Row(key));
        await s.Browser.Check(Selectors.Field("Sensitive", Selectors.Dialog), false);
        await s.Fill("Value", "fixture-public", Selectors.Dialog);
        await s.Response("configurationentries/" + row["id"]!.GetValue<string>(), "PUT", () => s.Click("Save", Selectors.Dialog));
        await s.Browser.Contains(Selectors.Row(key), "fixture-public");
        await s.Delete(key, "configurationentries", row["id"]!.GetValue<string>());
    });
}
