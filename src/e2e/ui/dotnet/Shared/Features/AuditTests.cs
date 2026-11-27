using System.Text.Json.Nodes;
using Xunit;

namespace ClassifiedAds.EndToEndTests.Features;

public sealed class AuditTests : TestBase
{
    [UiTheory("E2E_ALLOW_WRITES")]
    [Apps]
    public Task JsonFormatCopyCollapseAndFocus(string app) => Run(app, async s =>
    {
        await s.Create("products", new { name = s.Prefix + "-audit", code = "PWAUDIT", description = "Audit fixture" });
        await s.Open("Audit Logs");
        var trigger = "(" + Selectors.Action("View JSON") + ")[1]";
        await s.Browser.Click(trigger);
        await s.Browser.Contains(Selectors.Dialog, "JSON Data");
        await s.Click("Formatted", Selectors.Dialog);
        await s.Browser.Contains(Selectors.Dialog + "//*[@aria-label='JSON content']", "{");
        await s.Click("Copy JSON", Selectors.Dialog);
        await s.Browser.Contains(Selectors.Dialog + "//*[@role='status']", "Copied to clipboard");
        var copied = (await s.Browser.Evaluate("navigator.clipboard.readText()"))!.GetValue<string>();
        Assert.NotNull(JsonNode.Parse(copied));
        await s.Click("Tree", Selectors.Dialog);
        await s.Click("Collapse all", Selectors.Dialog);
        await Checks.Count(s, Selectors.Dialog + "//details[@open]", 0);
        await s.Browser.Key("Escape");
        await s.Browser.Hidden(Selectors.Dialog);
        await s.Browser.Focused(trigger);
    });

    [UiTheory("E2E_ALLOW_WRITES")]
    [Apps]
    public Task PaginationRequestsSelectedPage(string app) => Run(app, async s =>
    {
        for (var index = 0; index < 6; index++)
        {
            await s.Create("products", new { name = s.Prefix + "-page-" + index, code = "PWP" + index, description = "Pagination fixture" });
        }

        await s.Open("Audit Logs");
        var pagination = "(//*[contains(concat(' ',@class,' '),' pagination ')])[1]";
        foreach (var number in new[] { "2", "1" })
        {
            await s.Response("auditlogentries/paged", "GET", () => s.Click(number, pagination), number);
            await s.Browser.Contains(pagination + "//*[contains(concat(' ',@class,' '),' active ')]", number);
        }
    });
}
