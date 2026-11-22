using System.Text.Json;
using System.Text.RegularExpressions;
using ClassifiedAds.Mobile.Shared;

namespace ClassifiedAds.Mobile.Features.AuditLogs;

public static class AuditEntry
{
    public static View Create(JsonElement entry)
    {
        string Field(string name, string fallback) => entry.TryGetProperty(name, out var field) && !string.IsNullOrWhiteSpace(field.ToString()) ? field.ToString() : fallback;
        var rawDate = Field("createdDateTime", "");
        var timestamp = DateTimeOffset.TryParse(rawDate, out var date) ? date.ToLocalTime().ToString("g") : rawDate;
        var data = entry.TryGetProperty("log", out var log) ? log : JsonSerializer.SerializeToElement<object?>(null);
        if (data.ValueKind == JsonValueKind.String)
        {
            try
            {
                using var parsed = JsonDocument.Parse(data.GetString()!);
                data = parsed.RootElement.Clone();
            }
            catch (JsonException)
            {
                // Keep non-JSON events readable.
            }
        }

        var action = Ui.Text(Field("action", "Event"));
        action.FontAttributes = FontAttributes.Bold;
        var details = Ui.Stack();
        details.Spacing = 12;
        details.IsVisible = false;
        if (data.ValueKind == JsonValueKind.Object)
        {
            foreach (var field in data.EnumerateObject())
            {
                var label = Ui.Text(Regex.Replace(field.Name, "([a-z])([A-Z])", "$1 $2"));
                label.FontAttributes = FontAttributes.Bold;
                details.Add(Ui.Stack(label, Ui.Text(Display(field.Value))));
            }
        }
        else
        {
            details.Add(Ui.Text(Display(data)));
        }

        details.Add(Ui.Text("Full data"));
        var copy = Ui.Button("Copy JSON", "copy", () => Clipboard.SetTextAsync(JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true })));
        var json = JsonViewer.Create(data, includeCopy: false);
        json.IsVisible = false;
        Button? showJson = null;
        showJson = Ui.Button("Show JSON", "view", () =>
        {
            json.IsVisible = !json.IsVisible;
            showJson!.Text = json.IsVisible ? "Hide JSON" : "Show JSON";
            SemanticProperties.SetDescription(showJson, showJson.Text);
            return Task.CompletedTask;
        });
        details.Add(Ui.Actions(showJson, copy));
        details.Add(json);
        Button? toggle = null;
        toggle = Ui.Button("View event details", "view", () =>
        {
            details.IsVisible = !details.IsVisible;
            toggle!.Text = details.IsVisible ? "Hide event details" : "View event details";
            SemanticProperties.SetDescription(toggle, toggle.Text);
            return Task.CompletedTask;
        });
        var content = Ui.Stack(action, Ui.Text(timestamp), Ui.Text("By " + Field("userName", "Unknown user")), toggle, details);
        content.Spacing = 12;
        return Ui.Card(content);
    }

    private static string Display(JsonElement value)
    {
        if (value.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined || value.ToString() == "")
        {
            return "Not provided";
        }

        return value.ValueKind is JsonValueKind.Object or JsonValueKind.Array
            ? JsonSerializer.Serialize(value, new JsonSerializerOptions { WriteIndented = true })
            : value.ToString();
    }
}
