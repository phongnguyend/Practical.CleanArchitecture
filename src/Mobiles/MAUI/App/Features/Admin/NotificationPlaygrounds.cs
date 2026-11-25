using ClassifiedAds.Mobile.Shared;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ClassifiedAds.Mobile.Features.Admin;

public sealed class NotificationPlaygrounds : ContentView
{
    private readonly AppServices services;
    private readonly string channel;
    private readonly bool email;
    private readonly bool push;
    private string section = "notifications";
    private string Path => push ? "notifications/playground" : $"notifications/playgrounds/{channel}";
    private readonly VerticalStackLayout content = Ui.Stack();
    private readonly VerticalStackLayout history = Ui.Stack();
    private readonly Label status = Ui.Text("");
    private readonly Label feedback = Ui.Text("");
    private readonly Button queueButton;
    private bool busy;
    private string from = "";
    private string recipient = "";
    private string subject = "";
    private string message = "";

    public NotificationPlaygrounds(AppServices services, string channel, Action back)
    {
        this.services = services;
        this.channel = channel;
        email = channel == "email";
        push = channel == "mobilepush";
        Content = content;
        content.Add(Ui.Text(push ? "Mobile Push" : email ? "Email" : "SMS", true));
        content.Add(Ui.Button("Back", "back", () =>
        {
            back();
            return Task.CompletedTask;
        }));
        content.Add(Ui.Text(push ? "Queue a notification for your own registered devices." : $"Queue a message for delivery by the existing {(email ? "Email" : "SMS")} worker."));
        content.Add(status);
        if (email)
        {
            var sender = Ui.Field(content, "From", from, value => from = value);
            sender.MaxLength = 254;
            sender.Keyboard = Keyboard.Email;
        }

        if (!push)
        {
            var to = Ui.Field(content, email ? "To" : "Phone number", recipient, value => recipient = value);
            to.MaxLength = email ? 254 : 16;
            to.Keyboard = email ? Keyboard.Email : Keyboard.Telephone;
        }

        if (email || push)
        {
            Ui.Field(content, push ? "Title" : "Subject", subject, value => subject = value).MaxLength = push ? 100 : 200;
        }
        else
        {
            content.Add(Ui.Text("International format, for example +15551234567."));
        }

        content.Add(Ui.Text("Message"));
        var editor = new Editor
        {
            Placeholder = "Message",
            MaxLength = push ? 500 : email ? 10000 : 1600,
            MinimumHeightRequest = 120,
            AutoSize = EditorAutoSizeOption.TextChanges,
        };
        editor.TextChanged += (_, e) => message = e.NewTextValue ?? "";
        content.Add(editor);
        if (push)
        {
            content.Add(Ui.Text("Destination"));
            var picker = new Picker
            {
                Title = "Destination",
                ItemsSource = new[] { "notifications", "home", "products", "files", "users", "roles", "configurationentries", "auditlogentries" },
                SelectedItem = section,
            };
            picker.SelectedIndexChanged += (_, _) => section = picker.SelectedItem as string ?? "notifications";
            content.Add(picker);
        }

        queueButton = Ui.Button("Queue message", "notifications", Queue);
        queueButton.IsEnabled = false;
        content.Add(queueButton);
        content.Add(Ui.Button("Refresh queue", "refresh", () => Run(Refresh)));
        content.Add(feedback);
        content.Add(Ui.Text("Recent messages", true));
        content.Add(history);
        Loaded += async (_, _) => await Run(Refresh);
    }

    private Task Queue()
    {
        feedback.Text = "";
        if ((!push && string.IsNullOrWhiteSpace(recipient)) || string.IsNullOrWhiteSpace(message) || ((email || push) && string.IsNullOrWhiteSpace(subject)) || (email && string.IsNullOrWhiteSpace(from)))
        {
            feedback.Text = push ? "Title and message are required." : "Complete all required fields.";
            return Task.CompletedTask;
        }

        const string emailPattern = @"^[^\s@;]+@[^\s@;]+\.[^\s@;]+$";
        if (!push && (email ? !Regex.IsMatch(from.Trim(), emailPattern) || !Regex.IsMatch(recipient.Trim(), emailPattern) : !Regex.IsMatch(recipient.Trim(), @"^\+[1-9][0-9]{7,14}$")))
        {
            feedback.Text = email ? "Enter valid From and To email addresses." : "Enter a phone number in international format, for example +15551234567.";
            return Task.CompletedTask;
        }

        return Run(async () =>
        {
            object payload = push ? new { title = subject.Trim(), body = message.Trim(), section } : new { from = from.Trim(), recipient = recipient.Trim(), subject = subject.Trim(), body = message.Trim() };
            await services.Api.Write(Path, HttpMethod.Post, payload);
            feedback.Text = "Message queued successfully.";
            await Refresh();
        });
    }

    private async Task Run(Func<Task> action)
    {
        if (busy)
        {
            return;
        }

        busy = true;
        content.IsEnabled = false;
        feedback.Text = "";
        try
        {
            await action();
        }
        catch
        {
            feedback.Text = "Unable to access the queue. Check the fields and try again.";
        }
        finally
        {
            busy = false;
            content.IsEnabled = true;
        }
    }

    private async Task Refresh()
    {
        var data = await services.Api.Get<JsonElement>(Path);
        var provider = data.GetProperty("provider").GetString();
        status.Text = $"Provider: {provider}. " + (push ? $"Registered devices: {data.GetProperty("deviceCount")}. " : "") + (provider == "Fake" ? "Delivery is simulated; no message is sent." : "Queued messages may be delivered to the specified recipient.");
        queueButton.IsEnabled = true;
        history.Clear();
        var items = data.GetProperty("items");
        if (items.GetArrayLength() == 0)
        {
            history.Add(Ui.Text("No queued messages yet."));
        }

        foreach (var item in items.EnumerateArray())
        {
            history.Add(Ui.Stack(Ui.Text(item.GetProperty("title").GetString() ?? "", true),
                Ui.Text(push ? "" : $"To: {item.GetProperty("recipient")}"), Ui.Text(item.GetProperty("body").GetString() ?? ""),
                Ui.Text(push ? $"Pending: {item.GetProperty("pending")} / Completed: {item.GetProperty("completed")} / Failed: {item.GetProperty("failed")}" : $"{item.GetProperty("status")} / Attempts: {item.GetProperty("attempts")}")));
        }
    }
}
