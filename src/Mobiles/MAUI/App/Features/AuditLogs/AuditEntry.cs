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
        var actionType = Regex.Split(action.Text.Trim().ToUpperInvariant(), @"[_\s-]")[0];
        var (foreground, background) = actionType switch
        {
            "DELETE" or "DELETED" => ("#b91c1c", "#fee2e2"),
            "UPDATE" or "UPDATED" => ("#9a3412", "#ffedd5"),
            "ADD" or "ADDED" or "CREATE" or "CREATED" => ("#166534", "#dcfce7"),
            _ => ("#334155", "#e2e8f0"),
        };
        action.SetAppThemeColor(Label.TextColorProperty, Color.FromArgb(foreground), Color.FromArgb(foreground));
        var actionBadge = new Border
        {
            Content = action,
            BackgroundColor = Color.FromArgb(background),
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 6 },
            Padding = new Thickness(10, 4),
            HorizontalOptions = LayoutOptions.Start,
        };
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
        var content = Ui.Stack(actionBadge, Ui.Text(timestamp), Ui.Text("By " + Field("userName", "Unknown user")), toggle, details);
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
