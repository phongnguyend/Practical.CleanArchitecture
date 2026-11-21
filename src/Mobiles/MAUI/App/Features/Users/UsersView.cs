using ClassifiedAds.Mobile.Core.Models;
using ClassifiedAds.Mobile.Core.Network;
using ClassifiedAds.Mobile.Shared;

namespace ClassifiedAds.Mobile.Features.Users;

public sealed class UsersView(AppServices services) : RecordPage<User>(services, "Users", "users")
{
    private string? dateError, countError;
    protected override bool HasHistory => true;

    protected override View Form(User value, Action changed)
    {
        dateError = countError = null;
        var form = Ui.Stack();
        Ui.Field(form, "Username", value.UserName, text =>
        {
            value.UserName = text;
            changed();
        });
        Ui.Field(form, "Email", value.Email, text =>
        {
            value.Email = text;
            changed();
        }).Keyboard = Keyboard.Email;
        Ui.Toggle(form, "Email confirmed", value.EmailConfirmed, v =>
        {
            value.EmailConfirmed = v;
            changed();
        });
        Ui.Field(form, "Phone number", value.PhoneNumber, text =>
        {
            value.PhoneNumber = text;
            changed();
        });
        Ui.Toggle(form, "Phone confirmed", value.PhoneNumberConfirmed, v =>
        {
            value.PhoneNumberConfirmed = v;
            changed();
        });
        Ui.Toggle(form, "Two-factor enabled", value.TwoFactorEnabled, v =>
        {
            value.TwoFactorEnabled = v;
            changed();
        });
        Ui.Toggle(form, "Lockout enabled", value.LockoutEnabled, v =>
        {
            value.LockoutEnabled = v;
            changed();
        });
        var lockout = Ui.Field(form, "Lockout end (ISO 8601)", value.LockoutEnd?.ToString("O"), text =>
        {
            dateError = null;
            if (string.IsNullOrWhiteSpace(text))
            {
                value.LockoutEnd = null;
            }
            else if (DateTimeOffset.TryParse(text, out var date))
            {
                value.LockoutEnd = date;
            }
            else
            {
                dateError = "Enter a valid date and time.";
            }

            changed();
        });
        form.Add(Ui.DateTimePicker(lockout));
        Ui.Field(form, "Failed attempts", value.AccessFailedCount.ToString(), text =>
        {
            countError = null;
            if (int.TryParse(text, out var count) && count >= 0)
            {
                value.AccessFailedCount = count;
            }
            else
            {
                countError = "Failed attempts must be a nonnegative integer.";
            }

            changed();
        }).Keyboard = Keyboard.Numeric;
        return form;
    }

    protected override string? Validate(User value) => value.Validate() ?? dateError ?? countError;
    protected override string Subtitle(User value) => value.Email;
    protected override View Details(User value) => Ui.Stack(Ui.Text(value.UserName, true), Ui.Text("Email: " + value.Email), Ui.Text("Email confirmed: " + value.EmailConfirmed), Ui.Text("Phone: " + value.PhoneNumber), Ui.Text("Phone confirmed: " + value.PhoneNumberConfirmed), Ui.Text("Two-factor enabled: " + value.TwoFactorEnabled), Ui.Text("Lockout enabled: " + value.LockoutEnabled), Ui.Text("Lockout end: " + value.LockoutEnd), Ui.Text("Failed attempts: " + value.AccessFailedCount));
    protected override View ExtraActions(User value) => Ui.Stack(Ui.Button("Set password", "lock", async () =>
    {
        string password = "", confirmation = "";
        var dialog = new Dialog("Set password", async () =>
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new InvalidOperationException("Password is required.");
            }

            if (password != confirmation)
            {
                throw new InvalidOperationException("Passwords must match.");
            }

            await Services.Api.Write(ApiClient.PathFor("users", value.Id) + "/password", HttpMethod.Put, new { id = value.Id, password });
            return true;
        }, () => password.Length > 0 || confirmation.Length > 0);
        Ui.Field(dialog.Body, "Password", "", text => password = text, true);
        Ui.Field(dialog.Body, "Confirm password", "", text => confirmation = text, true);
        await dialog.Show();
    }), Ui.Button("Send reset email", "mail", () => SendEmail(value, "passwordresetemail", "Send password reset email")), Ui.Button("Send confirmation email", "mail", () => SendEmail(value, "emailaddressconfirmation", "Send confirmation email")));
    private async Task SendEmail(User value, string endpoint, string action)
    {
        if (!await Dialog.Confirm($"{action} to {value.Email}?"))
        {
            return;
        }

        await Services.Api.Write(ApiClient.PathFor("users", value.Id) + "/" + endpoint, HttpMethod.Post, new { id = value.Id });
        await Dialog.Message("Success", "Email requested.");
    }
}
