namespace ClassifiedAds.Mobile.Shared;

public sealed class Dialog : ContentPage
{
    public VerticalStackLayout Body { get; } = Ui.Stack();

    public Label Error { get; } = Ui.Text("");

    private readonly TaskCompletionSource<bool> completion = new();
    private readonly Func<bool>? dirty;
    private bool closing;
    private bool saving;
    private static INavigation Nav => Application.Current!.Windows[0].Page!.Navigation;

    public Dialog(string title, Func<Task<bool>>? save = null, Func<bool>? dirty = null, string accept = "Save")
    {
        this.dirty = dirty;
        BackgroundColor = Color.FromArgb("#990b1220");
        Error.TextColor = Color.FromArgb("#dc2626");
        var cancel = Ui.Button(save is null ? "Close" : "Cancel", "close", Cancel);
        var footer = save is null ? Ui.Actions(cancel) : Ui.Actions(cancel, Ui.Button(accept, "check", async () =>
        {
            Error.Text = "";
            saving = true;
            cancel.IsEnabled = false;
            Body.IsEnabled = false;
            try
            {
                if (await save())
                {
                    await Finish(true);
                }
            }
            catch (Exception ex)
            {
                Error.Text = ex.Message;
            }
            finally
            {
                saving = false;
                cancel.IsEnabled = true;
                Body.IsEnabled = true;
            }
        }));
        var grid = new Grid
        {
            RowDefinitions =
            {
                new(GridLength.Auto),
                new(GridLength.Star),
                new(GridLength.Auto),
                new(GridLength.Auto)
            },
            RowSpacing = 16
        };
        grid.Add(Ui.Text(title, true), 0, 0);
        grid.Add(new ScrollView { Content = Body }, 0, 1);
        grid.Add(Error, 0, 2);
        grid.Add(footer, 0, 3);
        var card = Ui.Card(grid);
        card.Margin = new Thickness(16, 48);
        card.MaximumWidthRequest = 600;
        Content = card;
    }

    public async Task<bool> Show()
    {
        await Nav.PushModalAsync(this);
        return await completion.Task;
    }

    public Task Close() => Finish(false);
    private async Task Finish(bool value)
    {
        if (closing)
        {
            return;
        }

        closing = true;
        await Nav.PopModalAsync();
        completion.TrySetResult(value);
    }

    private async Task Cancel()
    {
        if (saving)
        {
            return;
        }

        if (dirty?.Invoke() == true && !await Confirm("Discard unsaved changes?"))
        {
            return;
        }

        await Finish(false);
    }

    protected override bool OnBackButtonPressed()
    {
        _ = Cancel();
        return true;
    }

    public static async Task<bool> Confirm(string text)
    {
        var dialog = new Dialog("Confirm", () => Task.FromResult(true), accept: "Confirm");
        dialog.Body.Add(Ui.Text(text));
        return await dialog.Show();
    }

    public static async Task Message(string title, string message)
    {
        var dialog = new Dialog(title);
        dialog.Body.Add(Ui.Text(message));
        await dialog.Show();
    }
}
