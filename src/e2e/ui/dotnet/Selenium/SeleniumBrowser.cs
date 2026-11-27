using System.Drawing;
using System.Text.Json.Nodes;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

namespace ClassifiedAds.EndToEndTests;

public static class EngineFactory
{
    public static string Name => "selenium";

    public static UiBrowser Create() => new SeleniumBrowser();
}

public sealed class SeleniumBrowser : UiBrowser
{
    private ChromeDriver driver = null!;
    private readonly string downloads = Path.Combine(Path.GetTempPath(), "classifiedads-e2e-" + Guid.NewGuid().ToString("N"));

    public override Task Start()
    {
        Directory.CreateDirectory(downloads);
        var options = new ChromeOptions { AcceptInsecureCertificates = Settings.Enabled("E2E_IGNORE_HTTPS_ERRORS") };
        options.AddArgument("--start-maximized");
        if (Settings.Enabled("E2E_HEADLESS"))
        {
            options.AddArgument("--headless=new");
        }

        options.AddUserProfilePreference("download.default_directory", downloads);
        options.AddUserProfilePreference("download.prompt_for_download", false);
        options.AddUserProfilePreference("profile.default_content_setting_values.clipboard", 1);
        // Suppress browser password-manager UI only in ChromeDriver's temporary profile.
        options.AddUserProfilePreference("credentials_enable_service", false);
        options.AddUserProfilePreference("profile.password_manager_enabled", false);
        options.AddUserProfilePreference("profile.password_manager_leak_detection", false);
        driver = new ChromeDriver(options);
        driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(30);
        driver.Manage().Window.Maximize();
        driver.ExecuteCdpCommand("Browser.setPermission", new Dictionary<string, object>
        {
            ["permission"] = new Dictionary<string, object> { ["name"] = "clipboard-read" },
            ["setting"] = "granted",
        });
        return Task.CompletedTask;
    }

    public override Task Go(string url)
    {
        driver.Navigate().GoToUrl(url);
        return Task.CompletedTask;
    }

    public override Task Reload()
    {
        driver.Navigate().Refresh();
        return Task.CompletedTask;
    }

    private async Task<IWebElement> Find(string xpath, bool visible = true)
    {
        IWebElement? result = null;
        await Wait(() =>
        {
            try
            {
                result = driver.FindElements(By.XPath(xpath)).FirstOrDefault(element => !visible || (element.Displayed && element.Enabled));
                return Task.FromResult(result != null);
            }
            catch (StaleElementReferenceException)
            {
                return Task.FromResult(false);
            }
        }, "Element not ready: " + xpath);
        return result!;
    }

    public override async Task Click(string xpath)
    {
        var element = await Find(xpath);
        await Wait(() =>
        {
            try
            {
                element.Click();
                return Task.FromResult(true);
            }
            catch (ElementClickInterceptedException)
            {
                return Task.FromResult(false);
            }
            catch (StaleElementReferenceException)
            {
                element = driver.FindElement(By.XPath(xpath));
                return Task.FromResult(false);
            }
        }, "Element could not be clicked: " + xpath);
    }

    public override async Task Fill(string xpath, string value)
    {
        var element = await Find(xpath);
        new Actions(driver).Click(element).KeyDown(Keys.Control).SendKeys("a").KeyUp(Keys.Control).SendKeys(Keys.Backspace).Perform();
        if (value.Length > 0)
        {
            element.SendKeys(value);
        }
    }

    public override async Task Upload(string xpath, string path) => (await Find(xpath, false)).SendKeys(Path.GetFullPath(path));

    public override Task Select(string xpath, string value) => Click(xpath + "/option[@value=" + Selectors.Literal(value) + "]");

    public override Task Key(string key)
    {
        new Actions(driver).SendKeys(key switch { "Escape" => Keys.Escape, "Enter" => Keys.Enter, "Tab" => Keys.Tab, _ => key }).Perform();
        return Task.CompletedTask;
    }

    public override Task Resize(int width, int height)
    {
        driver.ExecuteCdpCommand("Emulation.setDeviceMetricsOverride", new Dictionary<string, object>
        {
            ["width"] = width, ["height"] = height, ["deviceScaleFactor"] = 1, ["mobile"] = false,
        });
        driver.Manage().Window.Size = new Size(width, height + 100);
        return Task.CompletedTask;
    }

    public override Task Media(string scheme)
    {
        driver.ExecuteCdpCommand("Emulation.setEmulatedMedia", new Dictionary<string, object>
        {
            ["features"] = new[] { new Dictionary<string, object> { ["name"] = "prefers-color-scheme", ["value"] = scheme } },
        });
        return Task.CompletedTask;
    }

    public override Task<JsonNode?> Evaluate(string expression)
    {
        var json = (string)driver.ExecuteAsyncScript("const done=arguments[arguments.length-1]; Promise.resolve(" + expression + ").then(value=>done(JSON.stringify(value ?? null)),error=>done(JSON.stringify({__e2eError:error.message}))); ");
        var result = JsonNode.Parse(json);
        if (result is JsonObject obj && obj["__e2eError"] != null)
        {
            throw new InvalidOperationException(obj["__e2eError"]!.GetValue<string>());
        }

        return Task.FromResult(result);
    }

    public override Task Screenshot(string path)
    {
        driver.GetScreenshot().SaveAsFile(path);
        return Task.CompletedTask;
    }

    public override async Task<string> Download(string xpath, string folder)
    {
        var before = Directory.GetFiles(downloads).ToHashSet();
        await Click(xpath);
        string? file = null;
        await Wait(() =>
        {
            file = Directory.GetFiles(downloads).FirstOrDefault(path => !before.Contains(path) && !path.EndsWith(".crdownload", StringComparison.OrdinalIgnoreCase) && !path.EndsWith(".tmp", StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(file != null && new FileInfo(file).Length > 0);
        }, "Expected completed download", 60);
        var destination = Path.Combine(folder, Path.GetFileName(file!));
        Directory.CreateDirectory(folder);
        File.Copy(file!, destination, true);
        return destination;
    }

    public override ValueTask DisposeAsync()
    {
        driver?.Quit();
        driver?.Dispose();
        // This directory is generated exclusively for this browser instance.
        if (Directory.Exists(downloads))
        {
            Directory.Delete(downloads, true);
        }

        return ValueTask.CompletedTask;
    }
}
