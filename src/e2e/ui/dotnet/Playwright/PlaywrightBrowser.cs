using System.Text.Json.Nodes;
using Microsoft.Playwright;

namespace ClassifiedAds.EndToEndTests;

public static class EngineFactory
{
    public static string Name => "playwright";

    public static UiBrowser Create() => new PlaywrightBrowser();
}

public sealed class PlaywrightBrowser : UiBrowser
{
    private IPlaywright playwright = null!;
    private IBrowser browser = null!;
    private IBrowserContext context = null!;
    private IPage page = null!;

    public override async Task Start()
    {
        playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        browser = await playwright.Chromium.LaunchAsync(new()
        {
            Headless = Settings.Enabled("E2E_HEADLESS"),
            Args = new[] { "--start-maximized" },
        });
        context = await browser.NewContextAsync(new()
        {
            IgnoreHTTPSErrors = Settings.Enabled("E2E_IGNORE_HTTPS_ERRORS"),
            AcceptDownloads = true,
            ViewportSize = ViewportSize.NoViewport,
            Permissions = new[] { "clipboard-read", "clipboard-write" },
        });
        page = await context.NewPageAsync();
        page.SetDefaultTimeout(20000);
    }

    public override async Task Go(string url) => await page.GotoAsync(url);

    public override async Task Reload() => await page.ReloadAsync();

    private ILocator Find(string xpath) => page.Locator("xpath=" + xpath).First;

    protected override bool IsNavigationTransition(Exception exception) => exception is PlaywrightException
        && exception.Message.Contains("Execution context was destroyed", StringComparison.Ordinal);

    public override Task Click(string xpath) => Find(xpath).ClickAsync();

    public override Task Fill(string xpath, string value) => Find(xpath).FillAsync(value);

    public override Task Upload(string xpath, string path) => Find(xpath).SetInputFilesAsync(path);

    public override async Task Select(string xpath, string value) => await Find(xpath).SelectOptionAsync(value);

    public override Task Key(string key) => page.Keyboard.PressAsync(key);

    public override Task Resize(int width, int height) => page.SetViewportSizeAsync(width, height);

    public override Task Media(string scheme) => page.EmulateMediaAsync(new() { ColorScheme = scheme == "dark" ? ColorScheme.Dark : ColorScheme.Light });

    public override async Task<JsonNode?> Evaluate(string expression)
    {
        var json = await page.EvaluateAsync<string>("async () => JSON.stringify((await (" + expression + ")) ?? null)");
        return JsonNode.Parse(json);
    }

    public override async Task Screenshot(string path) => await page.ScreenshotAsync(new() { Path = path, FullPage = true });

    public override async Task<string> Download(string xpath, string folder)
    {
        var download = await page.RunAndWaitForDownloadAsync(() => Click(xpath));
        var failure = await download.FailureAsync();
        if (failure != null)
        {
            throw new IOException("Download failed: " + failure);
        }

        var path = Path.Combine(folder, Path.GetFileName(download.SuggestedFilename));
        Directory.CreateDirectory(folder);
        await download.SaveAsAsync(path);
        return path;
    }

    public override async ValueTask DisposeAsync()
    {
        if (browser != null)
        {
            await browser.CloseAsync();
        }

        playwright?.Dispose();
    }
}
