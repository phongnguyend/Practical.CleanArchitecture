using System.Text.Json;

namespace ClassifiedAds.Mobile.Shared;

public static class AuditHistory
{
    public static View Create(JsonElement logs)
    {
        var content = Ui.Stack();
        if (logs.ValueKind != JsonValueKind.Array)
        {
            return JsonViewer.Create(logs);
        }

        foreach (var row in logs.EnumerateArray())
        {
            string Field(string name) => row.TryGetProperty(name, out var value) ? value.ToString() : "";
            var card = Ui.Stack(Ui.Text(Field("action").ToUpperInvariant(), true), Ui.Text(Field("userName") + " " + Field("createdDateTime")));
            if (row.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Object)
            {
                row.TryGetProperty("highLight", out var highlights);
                foreach (var field in data.EnumerateObject())
                {
                    var label = Ui.Text(field.Name + ": " + field.Value);
                    if (highlights.ValueKind == JsonValueKind.Object && highlights.TryGetProperty(field.Name, out var changed) && changed.ValueKind == JsonValueKind.True)
                    {
                        label.FontAttributes = FontAttributes.Bold;
                        label.SetAppThemeColor(Label.TextColorProperty, Color.FromArgb("#b45309"), Color.FromArgb("#fbbf24"));
                    }

                    card.Add(label);
                }
            }

            card.Add(JsonViewer.Create(row));
            content.Add(Ui.Card(card));
        }

        if (content.Count == 0)
        {
            content.Add(Ui.Text("No audit history"));
        }

        return content;
    }
}
