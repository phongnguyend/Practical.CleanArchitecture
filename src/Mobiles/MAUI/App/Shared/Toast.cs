namespace ClassifiedAds.Mobile.Shared;

public sealed class Toast : ContentView
{
    private readonly Label message = Ui.Text("");
    private int generation;
    public Toast()
    {
        IsVisible = false;
        VerticalOptions = LayoutOptions.End;
        Margin = new Thickness(16);
        var card = Ui.Card(Ui.Actions(message, Ui.Button("Dismiss", "close", () =>
        {
            generation++;
            IsVisible = false;
            return Task.CompletedTask;
        })));
        Content = card;
    }

    public async Task Show(string text)
    {
        var current = ++generation;
        message.Text = text;
        IsVisible = true;
        SemanticScreenReader.Announce(text);
        await Task.Delay(5000);
        if (current == generation)
        {
            IsVisible = false;
        }
    }
}
