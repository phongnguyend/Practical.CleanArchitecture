using Xunit;

namespace ClassifiedAds.EndToEndTests.Features;

public sealed class RolesTests : TestBase
{
    [UiTheory]
    [Apps]
    public Task RequiredFieldAndKeyboardDismissal(string app) => Run(app, s => Checks.RequiredModal(s, "Roles", "Add Role", "Name"));

    [UiTheory("E2E_ALLOW_WRITES")]
    [Apps]
    public Task CrudAndDetails(string app) => Run(app, async s =>
    {
        s.Own("roles");
        var name = s.Prefix + "-role";
        await s.Open("Roles");
        await s.Click("Add Role");
        await s.Fill("Name", name, Selectors.Dialog);
        await s.Response("roles", "POST", () => s.Click("Save", Selectors.Dialog));
        var row = await s.Find("roles", name);
        var id = row["id"]!.GetValue<string>();
        await s.Browser.Hidden(Selectors.Dialog);
        await s.Click(name, Selectors.Row(name));
        await s.Path("/roles/" + id);
        await s.Browser.Visible(Selectors.Text(name.ToUpperInvariant()));
        await s.Click("Back");
        await s.Click("Edit", Selectors.Row(name));
        var renamed = name + "-edited";
        await s.Fill("Name", renamed, Selectors.Dialog);
        await s.Response("roles/" + id, "PUT", () => s.Click("Save", Selectors.Dialog));
        await s.Browser.Hidden(Selectors.Dialog);
        await s.Delete(renamed, "roles", id);
    });
}
