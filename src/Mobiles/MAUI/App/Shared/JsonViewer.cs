using System.Text.Json;

namespace ClassifiedAds.Mobile.Shared;

public static class JsonViewer
{
    public static View Create(JsonElement value)
    {
        if (value.ValueKind == JsonValueKind.String)
        {
            try
            {
                using var parsed = JsonDocument.Parse(value.GetString()!);
                value = parsed.RootElement.Clone();
            }
            catch (JsonException)
            {
                // Non-JSON messages remain readable as text.
            }
        }

        var stack = Ui.Stack();
        stack.Add(Ui.Button("Copy JSON", "copy", () => Clipboard.SetTextAsync(JsonSerializer.Serialize(value, new JsonSerializerOptions { WriteIndented = true }))));
        Add(stack, "JSON", value, 0);
        return stack;
    }

    private static void Add(VerticalStackLayout parent, string name, JsonElement value, int depth)
    {
        if (value.ValueKind is not (JsonValueKind.Object or JsonValueKind.Array))
        {
            parent.Add(Ui.Text(name + ": " + value));
            return;
        }

        var children = Ui.Stack();
        children.Margin = new Thickness(12, 0, 0, 0);
        children.IsVisible = depth == 0;
        var loaded = false;
        void Load()
        {
            if (loaded)
            {
                return;
            }

            loaded = true;
            if (value.ValueKind == JsonValueKind.Object)
            {
                foreach (var p in value.EnumerateObject())
                {
                    Add(children, p.Name, p.Value, depth + 1);
                }
            }
            else
            {
                var i = 0;
                foreach (var v in value.EnumerateArray())
                {
                    Add(children, $"[{i++}]", v, depth + 1);
                }
            }
        }

        if (children.IsVisible)
        {
            Load();
        }

        parent.Add(Ui.Button(name, "view", () =>
        {
            Load();
            children.IsVisible = !children.IsVisible;
            return Task.CompletedTask;
        }));
        parent.Add(children);
    }
}
