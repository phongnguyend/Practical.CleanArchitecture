using System.Text.Json;
using System.Text.Json.Nodes;

namespace ClassifiedAds.EndToEndTests;

public abstract class UiBrowser : IAsyncDisposable
{
    public abstract Task Start();

    public abstract Task Go(string url);

    public abstract Task Reload();

    public abstract Task Click(string xpath);

    public abstract Task Fill(string xpath, string value);

    public abstract Task Upload(string xpath, string path);

    public abstract Task Select(string xpath, string value);

    public abstract Task Key(string key);

    public abstract Task Resize(int width, int height);

    public abstract Task Media(string scheme);

    public abstract Task<JsonNode?> Evaluate(string expression);

    public abstract Task Screenshot(string path);

    public abstract Task<string> Download(string xpath, string folder);

    public abstract ValueTask DisposeAsync();

    protected virtual bool IsNavigationTransition(Exception exception) => false;

    public static string Json(object? value) => JsonSerializer.Serialize(value);

    public static string Element(string xpath) => $"document.evaluate({Json(xpath)}, document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue";

    public static string Elements(string xpath) => $$"""
        Array.from((function*() {
            const result = document.evaluate({{Json(xpath)}}, document, null, XPathResult.ORDERED_NODE_SNAPSHOT_TYPE, null);
            for (let i = 0; i < result.snapshotLength; i++) {
                yield result.snapshotItem(i);
            }
        })())
        """;

    public async Task Wait(Func<Task<bool>> condition, string message, int seconds = 20)
    {
        var stop = DateTime.UtcNow.AddSeconds(seconds);
        while (DateTime.UtcNow < stop)
        {
            try
            {
                if (await condition())
                {
                    return;
                }
            }
            catch (Exception exception) when (IsNavigationTransition(exception))
            {
                // A navigation can replace the document between two readiness checks.
            }

            await Task.Delay(100);
        }

        throw new TimeoutException(message);
    }

    public Task Visible(string xpath) => Wait(async () => (await Evaluate($"{Elements(xpath)}.some(e => e.getClientRects().length > 0 && getComputedStyle(e).visibility !== 'hidden')"))!.GetValue<bool>(), "Element not visible: " + xpath);

    public Task Hidden(string xpath) => Wait(async () => !(await Evaluate($"{Elements(xpath)}.some(e => e.getClientRects().length > 0 && getComputedStyle(e).visibility !== 'hidden')"))!.GetValue<bool>(), "Element still visible: " + xpath);

    public async Task<string> Text(string xpath)
    {
        await Visible(xpath);
        return (await Evaluate($"{Element(xpath)}.textContent"))!.GetValue<string>();
    }

    public Task Attribute(string xpath, string name, string expected) => Wait(async () => (await Evaluate($"{Element(xpath)}?.getAttribute({Json(name)})"))?.GetValue<string>() == expected, $"Expected {name}={expected}: {xpath}");

    public Task Value(string xpath, string expected) => Wait(async () => (await Evaluate($"{Element(xpath)}?.value"))?.GetValue<string>() == expected, "Unexpected field value: " + xpath);

    public Task Contains(string xpath, string text) => Wait(async () => (await Evaluate($"{Element(xpath)}?.textContent"))?.GetValue<string>().Contains(text, StringComparison.OrdinalIgnoreCase) == true, "Text not found: " + text);

    public Task Focused(string xpath) => Wait(async () => (await Evaluate($"document.activeElement === {Element(xpath)}"))!.GetValue<bool>(), "Focus not restored: " + xpath);

    public async Task Check(string xpath, bool value)
    {
        if ((await Evaluate($"{Element(xpath)}.checked"))!.GetValue<bool>() != value)
        {
            await Click(xpath);
        }
    }
}

public static class Selectors
{
    public const string Nav = "//nav[@aria-label='Main navigation']";
    public const string Dialog = "//*[@role='dialog']";
    public const string Files = "//input[@type='file']";
    public const string Invalid = "//*[contains(concat(' ',normalize-space(@class),' '),' is-invalid ') or @aria-invalid='true']";

    public static string Literal(string value) => value.Contains('\'') ? "concat('" + value.Replace("'", "',\"'\",'") + "')" : "'" + value + "'";

    public static string Action(string name, string scope = "") => scope + "//*[self::a or self::button][normalize-space(.)=" + Literal(name) + " or @aria-label=" + Literal(name) + "]";

    public static string Field(string name, string scope = "") => scope + "//*[self::input or self::textarea or self::select][@aria-label=" + Literal(name) + " or @id=//label[normalize-space(.)=" + Literal(name) + "]/@for]";

    public static string Text(string value) => "//*[normalize-space(.)=" + Literal(value) + " and not(*[normalize-space(.)=" + Literal(value) + "])]";

    public static string Row(string value) => "//table/tbody/tr[td[normalize-space(.)=" + Literal(value) + " or starts-with(normalize-space(.)," + Literal(value + " (") + ")]]";
}
