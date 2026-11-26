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
        content.Add(Tab("Playgrounds", "configuration", "playgrounds", page != "admin"));
        if (page != "admin")
        {
            var tabs = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new(GridLength.Star), new(GridLength.Star), new(GridLength.Star)
                }
            };
            tabs.Add(Tab("Mobile Push", "notifications", "mobilepush", page == "mobilepush"), 0);
            tabs.Add(Tab("Email", "mail", "email", page == "email"), 1);
            tabs.Add(Tab("SMS", "notifications", "sms", page == "sms"), 2);
            content.Add(tabs);
        }

        if (page != "admin" && page != "playgrounds")
        {
            content.Add(new NotificationPlaygrounds(services, page));
        }
        else
        {
            content.Add(Ui.Text(page == "admin" ? "Admin" : "Playgrounds", true));
            content.Add(Ui.Text("Choose a playground to inspect its queue and compose a message."));
        }
    }

    private View Tab(string label, string icon, string target, bool selected)
    {
        var button = Ui.Button(label, icon, () => Navigate(target));
        button.BorderWidth = 0;
        button.CornerRadius = 0;
        button.Padding = new Thickness(6, 8);
        button.TextColor = selected ? Ui.Primary : Color.FromArgb("#172554");
        button.ContentLayout = new Button.ButtonContentLayout(Button.ButtonContentLayout.ImagePosition.Top, 4);
        SemanticProperties.SetHint(button, selected ? "Selected tab" : "Open tab");
        var underline = new BoxView
        {
            HeightRequest = 3,
            Color = selected ? Ui.Primary : Color.FromArgb("#94a3b8")
        };
        return new VerticalStackLayout { Spacing = 0, Children = { button, underline } };
    }
}
