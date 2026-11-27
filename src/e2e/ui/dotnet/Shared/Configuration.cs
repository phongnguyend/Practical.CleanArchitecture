using System.Reflection;
using Xunit;
using Xunit.Sdk;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace ClassifiedAds.EndToEndTests;

public static class Settings
{
    static Settings()
    {
        var file = Environment.GetEnvironmentVariable("E2E_ENV_FILE");
        if (!string.IsNullOrEmpty(file))
        {
            foreach (var line in File.ReadAllLines(file))
            {
                var text = line.Trim();
                var separator = text.IndexOf('=');
                if (separator > 0 && !text.StartsWith('#'))
                {
                    var key = text[..separator].Trim();
                    if (Environment.GetEnvironmentVariable(key) == null)
                    {
                        Environment.SetEnvironmentVariable(key, text[(separator + 1)..].Trim().Trim('"', '\''));
                    }
                }
            }
        }
    }

    public static string Get(string key, string fallback = "") => Environment.GetEnvironmentVariable(key) ?? fallback;

    public static bool Enabled(string key) => Get(key) == "1";

    public static string[] Apps => Get("E2E_APPS", "angular,reactjs,vuejs").Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

    public static string Url(string app) => Get("E2E_" + app.ToUpperInvariant().Replace('-', '_') + "_URL", app switch
    {
        "angular" => "http://localhost:4200",
        "reactjs" or "nextjs" => "http://localhost:3000",
        "vuejs" => "http://localhost:8080",
        "bff-reactjs" or "blazor-wasm" => "https://localhost:44348",
        "blazor-server" => "https://localhost:44331",
        _ => throw new ArgumentException("Unknown E2E_APPS entry: " + app),
    });

    public static bool HasHub(string app) => app is "angular" or "reactjs" or "vuejs" or "bff-reactjs";
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class UiTheoryAttribute : TheoryAttribute
{
    public UiTheoryAttribute(string gate = "")
    {
        if (gate.Length > 0 && !Settings.Enabled(gate))
        {
            Skip = "Requires " + gate + "=1. See the .NET UI test README.";
        }
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class AppsAttribute : DataAttribute
{
    private readonly string[] cases;

    public AppsAttribute(params string[] cases)
    {
        this.cases = cases;
    }

    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        foreach (var app in Settings.Apps)
        {
            _ = Settings.Url(app);
            foreach (var value in cases.Length == 0 ? new[] { "" } : cases)
            {
                yield return new object[] { app }.Concat(value.Length == 0 ? Array.Empty<object>() : value.Split('|').Cast<object>()).ToArray();
            }
        }
    }
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class HubTheoryAttribute : TheoryAttribute
{
    public HubTheoryAttribute()
    {
        if (!Settings.Apps.Any(Settings.HasHub))
        {
            Skip = "Selected applications do not implement a notification hub client.";
        }
    }
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class HubAppsAttribute : DataAttribute
{
    public override IEnumerable<object[]> GetData(MethodInfo testMethod) => Settings.Apps.Where(Settings.HasHub).Select(app => new object[] { app });
}
