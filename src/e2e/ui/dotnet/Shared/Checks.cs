using Xunit;

namespace ClassifiedAds.EndToEndTests;

public static class Checks
{
    public static Task Count(Scenario s, string xpath, int count) => s.Browser.Wait(async () => (await s.Browser.Evaluate(UiBrowser.Elements(xpath) + ".length"))!.GetValue<int>() == count, "Unexpected count: " + xpath);

    public static Task Invalid(Scenario s, string scope = "//body") => s.Browser.Wait(async () => (await s.Browser.Evaluate($"Array.from({UiBrowser.Element(scope)}.querySelectorAll('input:invalid, .is-invalid, [aria-invalid=true]')).some(e => e.getClientRects().length > 0)"))!.GetValue<bool>(), "Expected visible validation highlight");

    public static async Task RequiredModal(Scenario s, string section, string action, string field)
    {
        await s.Open(section);
        await s.Click(action);
        await s.Browser.Visible(Selectors.Dialog);
        await s.Click("Save", Selectors.Dialog);
        await s.Browser.Value(Selectors.Field(field, Selectors.Dialog), "");
        await Invalid(s, Selectors.Dialog);
        await s.Browser.Key("Escape");
        await s.Browser.Hidden(Selectors.Dialog);
        await s.Browser.Focused(Selectors.Action(action));
    }

    public static async Task ImportValidation(Scenario s, string section, string action)
    {
        await s.Open(section);
        await s.Click(action);
        await s.Click("Import", Selectors.Dialog);
        await Invalid(s, Selectors.Dialog);
        await s.Click("Cancel", Selectors.Dialog);
        await s.Browser.Hidden(Selectors.Dialog);
    }

    public static void Download(string file, string extension)
    {
        Assert.Equal(extension, Path.GetExtension(file).ToLowerInvariant());
        Assert.True(new FileInfo(file).Length > 0, "Downloaded file is empty.");
    }

    public static async Task ImportAndReset(Scenario s, string action, string file)
    {
        await s.Click(action);
        await s.Browser.Upload(Selectors.Dialog + Selectors.Files, file);
        await s.Click("Import", Selectors.Dialog);
        await s.Browser.Hidden(Selectors.Dialog);
        await s.Click(action);
        await s.Browser.Value(Selectors.Dialog + Selectors.Files, "");
        await s.Click("Cancel", Selectors.Dialog);
        await s.Browser.Hidden(Selectors.Dialog);
    }

    public static async Task History(Scenario s, string name, string resource, string id)
    {
        await s.Response(resource + "/" + id + "/auditlogs", "GET", () => s.Click("View Audit Logs", Selectors.Row(name)));
        await s.Browser.Contains(Selectors.Dialog, "Audit Logs");
        await s.Browser.Key("Escape");
        await s.Browser.Hidden(Selectors.Dialog);
    }

    public static Task NoOverflow(Scenario s) => s.Browser.Wait(async () => (await s.Browser.Evaluate("document.documentElement.scrollWidth <= innerWidth"))!.GetValue<bool>(), "Page overflows horizontally");
}
