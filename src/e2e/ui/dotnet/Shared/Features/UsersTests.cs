using Xunit;

namespace ClassifiedAds.EndToEndTests.Features;

public sealed class UsersTests : TestBase
{
    [UiTheory]
    [Apps]
    public Task RequiredUsername(string app) => Run(app, async s =>
    {
        await s.Open("Users");
        await s.Click("Add User");
        await s.Click("Save");
        await s.Path("/users/add");
        await Checks.Invalid(s);
    });

    [UiTheory("E2E_ALLOW_WRITES")]
    [Apps]
    public Task CrudAndAccountDialogsWithoutSendingEmails(string app) => Run(app, async s =>
    {
        s.Own("users");
        var name = s.Prefix + "-user";
        await s.Open("Users");
        await s.Click("Add User");
        await s.Fill("User Name", name);
        await s.Fill("Email", name + "@example.invalid");
        await s.Fill("Phone Number", "0123456789");
        await s.Response("users", "POST", () => s.Click("Save"));
        var row = await s.Find("users", name);
        var id = row["id"]!.GetValue<string>();
        await s.Path("/users/" + id);
        await s.Open("Users");
        await s.Click("Edit", Selectors.Row(name));
        await s.Browser.Value(Selectors.Field("User Name"), name);
        await s.Fill("Phone Number", "0987654321");
        await s.Response("users/" + id, "PUT", () => s.Click("Save"));
        await s.Path("/users/" + id);
        await s.Browser.Visible(Selectors.Text("0987654321"));
        foreach (var label in new[] { "Set Password", "Send Password Reset Email", "Send Email Address Confirmation Email" })
        {
            await s.Click(label);
            await s.Browser.Contains(Selectors.Dialog, label);
            await s.Browser.Key("Escape");
            await s.Browser.Hidden(Selectors.Dialog);
        }

        await s.Click("Back");
        await s.Delete(name, "users", id);
    });
}
