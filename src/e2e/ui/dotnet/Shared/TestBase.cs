using System.Runtime.CompilerServices;
using Xunit;

namespace ClassifiedAds.EndToEndTests;

public abstract class TestBase
{
    protected static async Task Run(string app, Func<Scenario, Task> test, [CallerMemberName] string name = "")
    {
        var directory = Path.GetFullPath(Path.Combine(Settings.Get("E2E_ARTIFACTS_DIR", "TestResults/ui"), EngineFactory.Name, app, name + "-" + Guid.NewGuid().ToString("N")[..8]));
        Directory.CreateDirectory(directory);
        await using var browser = EngineFactory.Create();
        await browser.Start();
        var scenario = new Scenario(browser, app, directory);
        try
        {
            await scenario.Login();
            await test(scenario);
        }
        catch
        {
            try
            {
                await browser.Screenshot(Path.Combine(directory, "failure.png"));
            }
            catch
            {
                // Preserve the original test failure if the browser has already closed.
            }

            throw;
        }
        finally
        {
            await scenario.Cleanup();
        }
    }
}
