using ClassifiedAds.Mobile.Shared;
using System.Text.Json;

namespace ClassifiedAds.Mobile.Features.Notifications;

public sealed class PushInbox : ContentView
{
    private readonly AppServices services;
    private readonly Func<string, Task> navigate;
    private readonly VerticalStackLayout content = Ui.Stack();
    private int page = 1;
    private bool busy;

    public PushInbox(AppServices services, Func<string, Task> navigate)
    {
        this.services = services;
        this.navigate = navigate;
        Content = content;
        Loaded += async (_, _) => await Refresh();
    }

    private async Task Refresh()
    {
        if (busy || !services.SignedIn)
        {
            return;
        }

        busy = true;
        try
        {
            content.Clear();
            content.Add(Ui.Text("Notification inbox", true));
            if (!PushNotifications.Configured)
            {
                content.Add(Ui.Text("Push notifications are not configured for this build."));
            }

            var enable = Ui.Button("Enable notifications", "notifications", services.Push.Enable);
            enable.IsEnabled = PushNotifications.Configured;
            content.Add(enable);
            var disable = Ui.Button("Disable notifications", "close", services.Push.Disable);
            disable.IsEnabled = PushNotifications.Configured;
            content.Add(disable);
            content.Add(Ui.Button("Refresh inbox", "refresh", Refresh));
            var status = await services.Api.Get<JsonElement>("notifications/status");
            if (status.GetProperty("allowTestMessages").GetBoolean())
            {
                content.Add(Ui.Button("Send test notification", "notifications", async () =>
                {
                    await services.Api.Write("notifications/test", HttpMethod.Post);
                    await Refresh();
                }));
            }

            var result = await services.Api.Get<JsonElement>($"notifications/inbox?page={page}");
            if (!services.SignedIn)
            {
                content.Clear();
                return;
            }

            var items = result.GetProperty("items");
            foreach (var item in items.EnumerateArray())
            {
                var id = item.GetProperty("id").GetString();
                var section = item.GetProperty("section").GetString() ?? "notifications";
                var card = Ui.Stack(Ui.Text(item.GetProperty("title").GetString() ?? "", true), Ui.Text(item.GetProperty("body").GetString() ?? ""));
                card.Add(Ui.Text(item.GetProperty("createdAt").GetDateTime().ToLocalTime().ToString("g") + (item.GetProperty("readAt").ValueKind == JsonValueKind.Null ? " / Unread" : " / Read")));
                card.Add(Ui.Button("Open notification", "view", async () =>
                {
                    await services.Api.Write($"notifications/inbox/{id}/read", HttpMethod.Put);
                    await navigate(section);
                }));
                content.Add(card);
            }

            if (items.GetArrayLength() == 0)
            {
                content.Add(Ui.Text("No saved notifications yet."));
            }

            var total = result.GetProperty("total").GetInt32();
            content.Add(Ui.Text($"Page {page} / {total} notifications"));
            var previous = Ui.Button("Previous", "back", async () =>
            {
                page--;
                await Refresh();
            });
            previous.IsEnabled = page > 1;
            var next = Ui.Button("Next", "next", async () =>
            {
                page++;
                await Refresh();
            });
            next.IsEnabled = page * 20 < total;
            content.Add(Ui.Actions(previous, next));
        }
        catch
        {
            content.Add(Ui.Text("Unable to load notifications. Refresh to try again."));
        }
        finally
        {
            busy = false;
        }
    }
}
