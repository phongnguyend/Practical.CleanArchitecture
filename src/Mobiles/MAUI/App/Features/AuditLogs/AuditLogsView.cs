using System.Text.Json;
using ClassifiedAds.Mobile.Core.Models;
using ClassifiedAds.Mobile.Shared;

namespace ClassifiedAds.Mobile.Features.AuditLogs;

public sealed record AuditPage(List<JsonElement> Items, int TotalItems);
public sealed class AuditLogsView : ContentView
{
    private readonly AppServices services;
    private readonly Pagination paging = new();
    private readonly VerticalStackLayout records = Ui.Stack();
    private readonly Label summary = Ui.Text(""), error = Ui.Text("");
    private readonly Entry search = new()
    {
        Placeholder = "Search current records"
    };
    private readonly Button previous, next, refresh;
    private List<JsonElement> items = [];
    private bool loading;
    public AuditLogsView(AppServices services)
    {
        this.services = services;
        refresh = Ui.Button("Refresh", "refresh", Load);
        previous = Ui.Button("Previous", "back", async () =>
        {
            paging.Previous();
            await Load();
        });
        next = Ui.Button("Next", "next", async () =>
        {
            paging.Next();
            await Load();
        });
        search.SetAppThemeColor(Entry.TextColorProperty, Color.FromArgb("#172554"), Color.FromArgb("#e2e8f0"));
        search.TextChanged += (_, _) => Render();
        summary.HorizontalTextAlignment = TextAlignment.Center;
        Content = new ScrollView
        {
            Content = Ui.Stack(Ui.Text("Audit logs", true), search, refresh, error, records, summary, Ui.Actions(previous, next))
        };
        Loaded += async (_, _) => await Load();
    }

    private async Task Load()
    {
        if (loading)
        {
            return;
        }

        loading = true;
        refresh.IsEnabled = previous.IsEnabled = next.IsEnabled = false;
        error.Text = "Loading...";
        try
        {
            var result = await services.Api.Get<AuditPage>($"auditlogentries/paged?page={paging.Page}&pageSize=5");
            items = result.Items;
            paging.SetTotal(result.TotalItems);
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

    private void Render()
    {
        records.Clear();
        foreach (var row in items.Where(x => x.ToString().Contains(search.Text ?? "", StringComparison.OrdinalIgnoreCase)))
        {
            records.Add(AuditEntry.Create(row));
        }

        if (records.Count == 0)
        {
            records.Add(Ui.Text("No audit logs found."));
        }

        summary.Text = paging.Summary;
        previous.IsEnabled = !loading && paging.HasPrevious;
        next.IsEnabled = !loading && paging.HasNext;
    }
}
