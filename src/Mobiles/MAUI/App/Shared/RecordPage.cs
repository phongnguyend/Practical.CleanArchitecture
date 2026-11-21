using ClassifiedAds.Mobile.Core.Models;
using ClassifiedAds.Mobile.Core.Network;
using System.Text.Json;

namespace ClassifiedAds.Mobile.Shared;

public abstract class RecordPage<T> : ContentView where T : Record, new()
{
    protected readonly AppServices Services;
    protected readonly Repository<T> Repository;
    protected readonly VerticalStackLayout Toolbar = Ui.Stack();
    private readonly VerticalStackLayout records = Ui.Stack();
    private readonly Label error = Ui.Text("");
    private readonly Entry search;
    private readonly Pagination paging = new();
    private readonly Label summary = Ui.Text("");
    private readonly Button previous, next, refresh;
    private List<T> rows = [];
    private bool loading;
    protected bool ShowImages = true;
    protected virtual bool HasHistory => false;

    protected RecordPage(AppServices services, string title, string resource)
    {
        Services = services;
        Repository = new(services.Api, resource);
        search = new Entry
        {
            Placeholder = "Search records"
        };
        search.SetAppThemeColor(Entry.TextColorProperty, Color.FromArgb("#172554"), Color.FromArgb("#e2e8f0"));
        search.TextChanged += (_, _) =>
        {
            paging.Reset();
            Render();
        };
        refresh = Ui.Button("Refresh", "refresh", Load);
        Toolbar.Add(Ui.Actions(Ui.Button(title == "Files" ? "Upload file" : "Add new", title == "Files" ? "upload" : "add", () => Edit(new T())), refresh));
        previous = Ui.Button("Previous", "back", () =>
        {
            paging.Previous();
            Render();
            return Task.CompletedTask;
        });
        next = Ui.Button("Next", "next", () =>
        {
            paging.Next();
            Render();
            return Task.CompletedTask;
        });
        summary.HorizontalTextAlignment = TextAlignment.Center;
        error.TextColor = Color.FromArgb("#dc2626");
        Content = new ScrollView
        {
            Content = Ui.Stack(Ui.Text(title, true), search, Toolbar, error, records, summary, Ui.Actions(previous, next))
        };
        Loaded += async (_, _) => await Load();
    }

    protected abstract View Form(T value, Action changed);
    protected abstract View Details(T value);
    protected abstract string Subtitle(T value);
    protected virtual View? ExtraActions(T value) => null;
    protected virtual Task Save(T value) => Repository.Save(value);
    protected virtual string? Validate(T value) => value.Validate();
    protected async Task Load()
    {
        if (loading)
        {
            return;
        }

        loading = true;
        refresh.IsEnabled = false;
        previous.IsEnabled = next.IsEnabled = false;
        error.Text = "Loading...";
        try
        {
            rows = await Repository.List();
            error.Text = "";
        }
        catch (Exception ex)
        {
            error.Text = ex.Message;
        }
        finally
        {
            loading = false;
            refresh.IsEnabled = true;
            Render();
        }
    }

    protected void Render()
    {
        var filtered = rows.Where(row => row.Title.Contains(search.Text ?? "", StringComparison.OrdinalIgnoreCase)).ToList();
        paging.SetTotal(filtered.Count);
        records.Clear();
        if (filtered.Count == 0)
        {
            records.Add(Ui.Text("No records found"));
        }

        foreach (var row in paging.Slice(filtered))
        {
            var card = Ui.Stack(Ui.Text(row.Title, true), Ui.Text(Subtitle(row)));
            if (row is Product p)
            {
                card.Add(Ui.Button($"Rating {p.StarRating:0.#} out of 5", "view", () => Dialog.Message("Rating", $"{p.Name}: {p.StarRating:0.#} out of 5")));
                card.Add(Ui.Text($"Price: {p.Price:0.00}"));
                if (ShowImages && Uri.TryCreate(p.ImageUrl, UriKind.Absolute, out var uri) && uri.Scheme == "https")
                {
                    card.Add(new Image { Source = p.ImageUrl, HeightRequest = 120, Aspect = Aspect.AspectFit });
                }
            }

            card.Add(Ui.Actions(Ui.Button("View", "view", async () =>
            {
                var fresh = await Repository.Get(row.Id);
                var dialog = new Dialog("Details");
                dialog.Body.Add(Details(fresh));
                await dialog.Show();
            }), Ui.Button("Edit", "edit", async () => await Edit(await Repository.Get(row.Id)))));
            var delete = Ui.Button("Delete", "delete", async () =>
            {
                if (!await Dialog.Confirm($"Delete {row.Title}? This cannot be undone."))
                {
                    return;
                }

                await Repository.Delete(row.Id);
                await Load();
            }, true);
            if (HasHistory)
            {
                card.Add(Ui.Actions(Ui.Button("History", "history", async () =>
                {
                    var logs = await Repository.History(row.Id);
                    var dialog = new Dialog("Audit logs");
                    dialog.Body.Add(AuditHistory.Create(logs));
                    await dialog.Show();
                }), delete));
            }
            else
            {
                card.Add(delete);
            }

            var extra = ExtraActions(row);
            if (extra is not null)
            {
                card.Add(extra);
            }

            records.Add(Ui.Card(card));
        }

        summary.Text = paging.Summary;
        previous.IsEnabled = !loading && paging.HasPrevious;
        next.IsEnabled = !loading && paging.HasNext;
    }

    private async Task Edit(T value)
    {
        var dirty = false;
        var dialog = new Dialog(string.IsNullOrEmpty(value.Id) ? "Add new" : "Edit", async () =>
        {
            var message = Validate(value);
            if (message is not null)
            {
                throw new InvalidOperationException(message);
            }

            await Save(value);
            return true;
        }, () => dirty);
        dialog.Body.Add(Form(value, () => dirty = true));
        if (await dialog.Show())
        {
            await Load();
        }
    }

    protected async Task Import(string endpoint, string title)
    {
        var file = await FilePicker.Default.PickAsync();
        if (file is null)
        {
            return;
        }

        if (!await Dialog.Confirm($"Process {file.FileName}? Import reads records; it does not save them."))
        {
            return;
        }

        await using var stream = await file.OpenReadAsync();
        var result = await Services.Api.Upload(endpoint, stream, file.FileName);
        using var json = JsonDocument.Parse(result);
        var dialog = new Dialog(title);
        dialog.Body.Add(Ui.Text("Import processed. Records have not been saved."));
        dialog.Body.Add(JsonViewer.Create(json.RootElement.Clone()));
        await dialog.Show();
        await Load();
    }
}
