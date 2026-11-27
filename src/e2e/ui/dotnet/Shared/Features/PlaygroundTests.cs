using Xunit;

namespace ClassifiedAds.EndToEndTests.Features;

public sealed class PlaygroundTests : TestBase
{
    [UiTheory]
    [Apps]
    public Task NestedTabsAndReload(string app) => Run(app, async s =>
    {
        var admin = "//nav[@aria-label='Admin sections']";
        var channels = "//nav[@aria-label='Playground channels']";
        foreach (var label in new[] { "Mobile Push", "Email", "SMS" })
        {
            await s.Playground(label);
            await s.Browser.Wait(async () => (await s.Browser.Evaluate($"{UiBrowser.Element(Selectors.Action("Playgrounds", admin))}.classList.contains('active')"))!.GetValue<bool>(), "Playgrounds subtab is not active");
            await s.Browser.Attribute(Selectors.Action(label, channels), "aria-current", "page");
            await Checks.Count(s, channels + "//a[contains(concat(' ',@class,' '),' active ')]", 1);
            await Checks.Count(s, channels + "//a", 3);
        }

        await s.Browser.Reload();
        await s.Browser.Attribute(Selectors.Action("SMS", channels), "aria-current", "page");
    });

    [UiTheory]
    [Apps("Email|10000", "SMS|1600", "Mobile Push|500")]
    public Task RequiredFieldsAndRecipientValidation(string app, string channel, string length) => Run(app, async s =>
    {
        await s.Playground(channel);
        await s.Browser.Attribute(Selectors.Field("Message"), "maxlength", length);
        await s.Click("Queue message");
        await s.Browser.Visible(Selectors.Text(channel == "Mobile Push" ? "Title and message are required." : "Complete all required fields."));
        if (channel == "Mobile Push")
        {
            await s.Browser.Attribute(Selectors.Field("Title"), "maxlength", "100");
        }
        else
        {
            if (channel == "Email")
            {
                await s.Fill("From", "sender@example.com");
                await s.Fill("Subject", "Validation only");
            }

            await s.Fill(channel == "Email" ? "To" : "Phone number", "invalid");
            await s.Fill("Message", "Do not send this invalid request.");
            await s.Click("Queue message");
            await s.Browser.Contains("//*[@role='alert']", channel == "Email" ? "Enter valid From and To email addresses." : "Enter a phone number in international format");
        }

        await s.Browser.Visible(Selectors.Text("Recent messages"));
    });

    [UiTheory("E2E_ALLOW_WRITES")]
    [Apps("Email", "SMS", "Mobile Push")]
    public Task QueueThroughFakeProviderAndRefresh(string app, string channel) => Run(app, async s =>
    {
        await s.Playground(channel);
        var provider = "//*[@role='status'][contains(.,'Provider:')]";
        await s.Browser.Visible(provider);
        Assert.Contains("Provider: Fake", await s.Browser.Text(provider));
        var message = s.Prefix + "-" + channel;
        if (channel == "Mobile Push")
        {
            await s.Fill("Title", message);
            await s.Browser.Select(Selectors.Field("Destination"), "notifications");
        }
        else
        {
            if (channel == "Email")
            {
                await s.Fill("From", "sender@example.com");
                await s.Fill("Subject", "Playground verification");
            }

            await s.Fill(channel == "Email" ? "To" : "Phone number", channel == "Email" ? "recipient@example.com" : "+15551234567");
        }

        await s.Fill("Message", message);
        await s.Click("Queue message");
        await s.Browser.Contains("//*[@role='status'][contains(.,'Message queued.') ]", "Message queued. ID:");
        await s.Click("Refresh queue");
        await s.Browser.Visible("//article[contains(.," + Selectors.Literal(message) + ")]");
    });
}
