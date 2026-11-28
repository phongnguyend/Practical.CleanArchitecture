using ClassifiedAds.Mobile.Shared;

namespace ClassifiedAds.Mobile.Features.Roles;

public sealed class MembershipView : ContentView
{
    private sealed class Item
    {
        public string Id { get; set; } = "";

        public string Name { get; set; } = "";

        public string UserName { get; set; } = "";

        public string Email { get; set; } = "";
    }

    private readonly AppServices services;
    private readonly string? roleId;
    private readonly string? userId;
    private readonly VerticalStackLayout root = Ui.Stack();
    private readonly VerticalStackLayout members = Ui.Stack();
    private readonly Label status = Ui.Text("");
    private readonly Picker picker = new() { Title = "User to assign", ItemDisplayBinding = new Binding(nameof(Item.UserName)) };
    private readonly Entry search = new() { Placeholder = "Search users" };
    private readonly Button assign;
    private readonly Button refresh;
    private List<Item> items = [];
    private List<Item> users = [];
    private bool busy;
    private bool initialized;

    public MembershipView(AppServices services, string? roleId = null, string? userId = null)
    {
        this.services = services;
        this.roleId = roleId;
        this.userId = userId;
        root.Spacing = 16;
        refresh = Ui.Button(roleId != null ? "Refresh users" : "Refresh roles", "refresh", Load);
        refresh.Text = "";
        refresh.WidthRequest = 48;
        var header = new Grid
        {
            ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
            ColumnSpacing = 12,
        };
        var heading = Ui.Text(roleId != null ? "Role users" : "Assigned roles", true);
        heading.VerticalOptions = LayoutOptions.Center;
        header.Add(heading);
        header.Add(refresh, 1);
        root.Add(header);
        root.Add(status);
        assign = Ui.Button("Assign user", "add", async () =>
        {
            if (picker.SelectedItem is Item user)
            {
                await Change(user, true);
            }
        });
        assign.IsEnabled = false;
        if (roleId != null)
        {
            SemanticProperties.SetDescription(search, "Search users");
            SemanticProperties.SetDescription(picker, "User to assign");
            search.TextChanged += (_, _) => Filter();
            picker.SelectedIndexChanged += (_, _) => assign.IsEnabled = !busy && picker.SelectedItem != null;
            root.Add(Ui.Text("Search by username or email to assign someone to this role."));
            root.Add(Ui.Text("Search users"));
            root.Add(search);
            root.Add(Ui.Text("User to assign"));
            root.Add(picker);
            root.Add(assign);
        }
        root.Add(members);
        root.Insert(0, Divider());
        root.Margin = new Thickness(0, 16, 0, 0);
        Content = root;
        Loaded += async (_, _) =>
        {
            if (!initialized)
            {
                initialized = true;
                await Load();
            }
        };
    }

    private static BoxView Divider()
    {
        var divider = new BoxView { HeightRequest = 1, HorizontalOptions = LayoutOptions.Fill };
        divider.SetAppThemeColor(BoxView.ColorProperty, Color.FromArgb("#dce3ed"), Color.FromArgb("#414954"));
        return divider;
    }

    private void Filter()
    {
        picker.ItemsSource = users.Where(user => !items.Any(item => item.Id == user.Id)
            && $"{user.UserName} {user.Email}".Contains(search.Text ?? "", StringComparison.OrdinalIgnoreCase)).ToList();
        picker.SelectedIndex = -1;
    }

    private void SetBusy(bool value)
    {
        busy = value;
        refresh.IsEnabled = !value;
        search.IsEnabled = !value;
        picker.IsEnabled = !value;
        assign.IsEnabled = !value && picker.SelectedItem != null;
        members.IsEnabled = !value;
    }

    private async Task Load()
    {
        SetBusy(true);
        status.Text = "Loading...";
        try
        {
            items = await services.Api.Get<List<Item>>(roleId != null ? $"roles/{roleId}/users" : $"users/{userId}/roles");
            if (roleId != null)
            {
                users = await services.Api.Get<List<Item>>("users");
                Filter();
            }
            members.Clear();
            foreach (var item in items)
            {
                var card = Ui.Stack(Ui.Text(roleId != null ? item.UserName : item.Name));
                card.Spacing = 12;
                if (roleId != null)
                {
                    card.Add(Ui.Text(item.Email));
                    var remove = Ui.Button("Remove", "delete", async () =>
                    {
                        if (await Dialog.Confirm($"Remove {item.UserName} from this role?"))
                        {
                            await Change(item, false);
                        }
                    }, true);
                    SemanticProperties.SetDescription(remove, $"Remove {item.UserName} from role");
                    card.Add(remove);
                }
                card.Insert(0, Divider());
                members.Add(card);
            }
            status.Text = items.Count == 0 ? (roleId != null ? "No users assigned." : "No roles assigned.") : "";
        }
        catch (Exception)
        {
            status.Text = "Unable to load role membership. Please try again.";
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async Task Change(Item user, bool assigned)
    {
        if (busy)
        {
            return;
        }
        SetBusy(true);
        try
        {
            await services.Api.Write($"roles/{roleId}/users/{user.Id}", assigned ? HttpMethod.Put : HttpMethod.Delete);
            await Load();
        }
        catch (Exception)
        {
            status.Text = "Unable to update role membership. Refresh and try again.";
        }
        finally
        {
            SetBusy(false);
        }
    }
}
