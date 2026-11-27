using System.Text.Json.Nodes;
using Xunit;

namespace ClassifiedAds.EndToEndTests;

public sealed class Scenario
{
    private readonly HashSet<string> resources = new(StringComparer.OrdinalIgnoreCase);

    public Scenario(UiBrowser browser, string app, string directory)
    {
        Browser = browser;
        App = app;
        Directory = directory;
    }

    public UiBrowser Browser { get; }

    public string App { get; }

    public string Directory { get; }

    public string Prefix { get; } = "e2e-" + Guid.NewGuid().ToString("N")[..10];

    public async Task Login()
    {
        Assert.False(string.IsNullOrWhiteSpace(Settings.Get("E2E_USERNAME")), "Set E2E_USERNAME (or E2E_ENV_FILE).");
        Assert.False(string.IsNullOrWhiteSpace(Settings.Get("E2E_PASSWORD")), "Set E2E_PASSWORD (or E2E_ENV_FILE).");
        await Browser.Go(Settings.Url(App).TrimEnd('/') + (App.StartsWith("blazor", StringComparison.Ordinal) ? "/" : "/home"));
        await Browser.Click(Selectors.Action("Login", Selectors.Nav));
        await Browser.Fill(Selectors.Field("Username"), Settings.Get("E2E_USERNAME"));
        await Browser.Fill(Selectors.Field("Password"), Settings.Get("E2E_PASSWORD"));
        await Browser.Click(Selectors.Action("Login"));
        await Browser.Visible(Selectors.Action("Logout", Selectors.Nav));
        await Browser.Visible(Selectors.Text("Phong Nguyen"));
        // Start scenarios on a new document after the OIDC callback, as the Node fixture does.
        await Browser.Go(Settings.Url(App).TrimEnd('/') + (App.StartsWith("blazor", StringComparison.Ordinal) ? "/" : "/home"));
        await Browser.Visible(Selectors.Action("Logout", Selectors.Nav));
        await Browser.Visible(Selectors.Text("Phong Nguyen"));
    }

    public async Task Open(string section)
    {
        var resource = section switch
        {
            "Products" => "products",
            "Files" => "files",
            "Users" => "users",
            "Roles" => "roles",
            "Settings" => "configurationentries",
            "Audit Logs" => "auditlogentries/paged",
            _ => null,
        };
        if (resource == null)
        {
            await Browser.Click(Selectors.Action(section, Selectors.Nav));
        }
        else
        {
            await Response(resource, "GET", () => Browser.Click(Selectors.Action(section, Selectors.Nav)));
        }
        var path = section switch
        {
            "Home" => "/home",
            "Audit Logs" => "/auditlogs",
            _ => "/" + section.ToLowerInvariant(),
        };
        await Browser.Wait(async () =>
        {
            var current = (await Browser.Evaluate("location.pathname"))!.GetValue<string>().TrimEnd('/');
            return current == path || (section == "Home" && current.Length == 0);
        }, "Navigation did not reach " + section);
        if (App.StartsWith("blazor", StringComparison.Ordinal) && section is not "Home" and not "Admin")
        {
            await Browser.Visible("//*[@data-page-loaded='true']");
        }
    }

    public Task Click(string name, string scope = "") => Browser.Click(Selectors.Action(name, scope));

    public Task Fill(string label, string value, string scope = "") => Browser.Fill(Selectors.Field(label, scope), value);

    public Task Path(string suffix) => Browser.Wait(async () => (await Browser.Evaluate("location.pathname"))!.GetValue<string>().EndsWith(suffix, StringComparison.OrdinalIgnoreCase), "Expected URL ending " + suffix);

    public async Task Theme(string theme)
    {
        await Browser.Click("//*[@aria-label and contains(@aria-label,'Choose appearance')]");
        await Click(theme);
        if (theme != "System")
        {
            await Browser.Attribute("//html", "data-bs-theme", theme.ToLowerInvariant());
        }
    }

    public async Task Playground(string channel)
    {
        await Open("Admin");
        await Click("Playgrounds", "//nav[@aria-label='Admin sections']");
        await Click(channel, "//nav[@aria-label='Playground channels']");
        await Browser.Visible("//*[self::h1 or self::h2 or self::h3][normalize-space(.)=" + Selectors.Literal(channel) + "]");
    }

    public string Fixture(string name, byte[] bytes)
    {
        var path = System.IO.Path.Combine(Directory, name);
        File.WriteAllBytes(path, bytes);
        return path;
    }

    public Task<string> Download(string action) => Browser.Download(Selectors.Action(action), Directory);

    public async Task<JsonNode> Api(string resource, string method = "GET", object? data = null)
    {
        var cookie = App is "nextjs" or "bff-reactjs";
        var origin = cookie ? Settings.Url(App).TrimEnd('/') + "/api/" : Settings.Get("E2E_API_URL", "https://localhost:44312/api/").TrimEnd('/') + "/";
        var result = await Browser.Evaluate($$"""
            (async () => {
              const headers = {'Content-Type':'application/json'};
              if ({{UiBrowser.Json(cookie)}}) {
                if ({{UiBrowser.Json(App == "bff-reactjs")}}) {
                  await fetch('/userinfor');
                  const token = document.cookie.split('; ').find(v => v.startsWith('PHONG-XSRF-TOKEN='));
                  if (!token) {
                    throw new Error('BFF anti-forgery cookie is missing');
                  }
                  headers['X-XSRF-TOKEN'] = decodeURIComponent(token.substring(token.indexOf('=')+1));
                }
              } else {
                let token = {{UiBrowser.Json(Settings.Get("E2E_API_TOKEN"))}};
                for (const key of Object.keys(localStorage)) {
                  if (key.startsWith('oidc.user:')) {
                    token = JSON.parse(localStorage.getItem(key)).access_token || token;
                  }
                }
                if (!token) {
                  throw new Error('No API token. For Blazor write fixtures set E2E_API_TOKEN.');
                }
                headers.Authorization = 'Bearer ' + token;
              }
              const response = await fetch({{UiBrowser.Json(origin + resource)}}, {
                method: {{UiBrowser.Json(method)}}, headers, credentials: 'include',
                body: {{UiBrowser.Json(data == null ? null : System.Text.Json.JsonSerializer.Serialize(data))}} || undefined
              });
              return {status: response.status, text: await response.text()};
            })()
            """);
        return result!;
    }

    public void Own(string resource)
    {
        Assert.True(Settings.Enabled("E2E_ALLOW_WRITES"), "Write fixtures require E2E_ALLOW_WRITES=1.");
        resources.Add(resource);
    }

    public async Task<JsonNode> Create(string resource, object data)
    {
        Own(resource);
        var response = await Api(resource, "POST", data);
        Assert.InRange(response["status"]!.GetValue<int>(), 200, 299);
        var row = JsonNode.Parse(response["text"]!.GetValue<string>())!;
        Assert.True(IsOwned(row), "Created row does not belong to this test.");
        return row;
    }

    public async Task<JsonNode> Find(string resource, string name)
    {
        JsonNode? found = null;
        await Browser.Wait(async () =>
        {
            var response = await Api(resource);
            Assert.InRange(response["status"]!.GetValue<int>(), 200, 299);
            var rows = JsonNode.Parse(response["text"]!.GetValue<string>())!.AsArray();
            found = rows.FirstOrDefault(row => row != null && new[] { "name", "key", "userName" }.Any(key => row[key]?.GetValue<string>() == name));
            return found != null;
        }, "Created fixture not found: " + resource);
        return found!;
    }

    private bool IsOwned(JsonNode row) => new[] { "name", "key", "userName" }.Any(key => row[key]?.GetValue<string>().StartsWith(Prefix, StringComparison.Ordinal) == true);

    public async Task Cleanup()
    {
        var errors = new List<string>();
        foreach (var resource in resources.Reverse())
        {
            try
            {
                var response = await Api(resource);
                Assert.InRange(response["status"]!.GetValue<int>(), 200, 299);
                foreach (var row in JsonNode.Parse(response["text"]!.GetValue<string>())!.AsArray())
                {
                    if (row == null || !IsOwned(row) || row["id"] == null)
                    {
                        continue;
                    }

                    var id = row["id"]!.GetValue<string>();
                    var current = await Api(resource + "/" + id);
                    if (current["status"]!.GetValue<int>() is 204 or 404 || string.IsNullOrWhiteSpace(current["text"]!.GetValue<string>()))
                    {
                        continue;
                    }

                    Assert.InRange(current["status"]!.GetValue<int>(), 200, 299);
                    Assert.True(IsOwned(JsonNode.Parse(current["text"]!.GetValue<string>())!), "Cleanup ownership check failed.");
                    var removed = await Api(resource + "/" + id, "DELETE");
                    Assert.True(removed["status"]!.GetValue<int>() is >= 200 and < 300 or 404, "Cleanup delete failed.");
                }
            }
            catch (Exception exception)
            {
                errors.Add(resource + ": " + exception.Message);
            }
        }

        Assert.True(errors.Count == 0, string.Join(Environment.NewLine, errors));
    }

    public async Task Delete(string name, string resource, string id)
    {
        await Click("Delete", Selectors.Row(name));
        await Browser.Contains(Selectors.Dialog, name);
        await Response(resource + "/" + id, "DELETE", () => Browser.Click(Selectors.Dialog + "//*[self::button or self::a][normalize-space(.)='Yes' or normalize-space(.)='Delete']"));
        await Browser.Hidden(Selectors.Dialog);
        await Browser.Hidden(Selectors.Row(name));
    }

    public async Task Response(string resource, string method, Func<Task> action, string? query = null)
    {
        if (App.StartsWith("blazor", StringComparison.Ordinal))
        {
            await action();
            return;
        }

        // Observe browser fetch/XHR responses without mocking or changing their payloads.
        await Browser.Evaluate("""
            (() => {
              window.__e2eResponses = [];
              if (!window.__e2eObserved) {
                window.__e2eObserved = true;
                const originalFetch = window.fetch;
                window.fetch = async function(...args) {
                  const response = await originalFetch.apply(this,args);
                  const method = args[1]?.method || args[0]?.method || 'GET';
                  window.__e2eResponses.push({url:response.url,method:method.toUpperCase(),status:response.status});
                  return response;
                };
                const open = XMLHttpRequest.prototype.open;
                XMLHttpRequest.prototype.open = function(method,url,...args) {
                  this.addEventListener('loadend', () => window.__e2eResponses.push({url:this.responseURL,method:method.toUpperCase(),status:this.status}));
                  return open.call(this,method,url,...args);
                };
              }
              return true;
            })()
            """);
        await action();
        JsonNode? response = null;
        await Browser.Wait(async () =>
        {
            response = await Browser.Evaluate($"(window.__e2eResponses || []).find(r => new URL(r.url,location.href).pathname.toLowerCase().replace(/\\/$/,'').endsWith('/api/' + {UiBrowser.Json(resource.ToLowerInvariant())}) && r.method === {UiBrowser.Json(method)} && ![301,302,303,307,308].includes(r.status) && ({UiBrowser.Json(query)} === null || new URL(r.url).searchParams.get('page') === {UiBrowser.Json(query)})) || null");
            return response != null;
        }, "Expected API response: " + method + " " + resource);
        Assert.InRange(response!["status"]!.GetValue<int>(), 200, 299);
    }
}
