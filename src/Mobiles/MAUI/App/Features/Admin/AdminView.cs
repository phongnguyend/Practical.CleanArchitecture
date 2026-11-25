using ClassifiedAds.Mobile.Shared;

namespace ClassifiedAds.Mobile.Features.Admin;

public sealed class AdminView : ContentView
{
    private readonly AppServices services;
    private readonly VerticalStackLayout content = Ui.Stack();
    private string page = "admin";

    public AdminView(AppServices services)
    {
        this.services = services;
        Content = new ScrollView
        {
            Content = content
        };
        Render();
    }

    private Task Navigate(string target)
    {
        page = target;
        Render();
        return Task.CompletedTask;
    }

    private void Render()
    {
        content.Clear();
        if (page != "admin" && page != "playgrounds")
        {
            content.Add(new NotificationPlaygrounds(services, page, () =>
            {
                page = "playgrounds";
                Render();
            }));
            return;
        }

        content.Add(Ui.Text(page == "admin" ? "Admin" : "Playgrounds", true));
        if (page == "admin")
        {
            content.Add(Ui.Button("Playgrounds", "configuration", () => Navigate("playgrounds")));
        }
        else
        {
            content.Add(Ui.Button("Back", "back", () => Navigate("admin")));
            content.Add(Ui.Button("Mobile Push", "notifications", () => Navigate("mobilepush")));
            content.Add(Ui.Button("Email", "mail", () => Navigate("email")));
            content.Add(Ui.Button("SMS", "notifications", () => Navigate("sms")));
        }
    }
}
