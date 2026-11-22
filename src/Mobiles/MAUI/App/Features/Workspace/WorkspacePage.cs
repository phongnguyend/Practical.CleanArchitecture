using System.Globalization;
using ClassifiedAds.Mobile.Features.Products;
using ClassifiedAds.Mobile.Features.Files;
using ClassifiedAds.Mobile.Features.Users;
using ClassifiedAds.Mobile.Features.Roles;
using ClassifiedAds.Mobile.Features.Configuration;
using ClassifiedAds.Mobile.Features.AuditLogs;
using ClassifiedAds.Mobile.Shared;

namespace ClassifiedAds.Mobile.Features.Workspace;

public sealed class WorkspacePage : ContentPage
{
    private AppServices services = new();
    private readonly ContentView body = new();
    private readonly Toast toast = new();
    private readonly VerticalStackLayout header = Ui.Stack();
    private Label clock = Ui.Text(""), status = Ui.Text("Disconnected");
    private VerticalStackLayout messages = Ui.Stack();
    private IDispatcherTimer? timer;
    private bool initialized;
    public WorkspacePage()
    {
        this.SetAppThemeColor(BackgroundColorProperty, Color.FromArgb("#f3f6fb"), Color.FromArgb("#0b1220"));
        var grid = new Grid
        {
            Padding = 16,
            RowSpacing = 16,
            RowDefinitions =
            {
                new(GridLength.Auto),
                new(GridLength.Star)
            }
        };
        grid.Add(header);
        grid.Add(body, 0, 1);
        var overlay = new Grid();
        overlay.Add(grid);
        overlay.Add(toast);
        Content = overlay;
        services.NotificationsChanged += UpdateNotifications;
        services.MessageReceived += ShowToast;
        Loaded += async (_, _) =>
        {
            if (initialized)
            {
                return;
            }

            initialized = true;
            try
            {
                await services.Restore();
            }
            catch (Exception ex)
            {
                await Dialog.Message("Session", ex.Message);
            }

            Build();
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        timer ??= Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromSeconds(1);
        timer.Tick -= Tick;
        timer.Tick += Tick;
        timer.Start();
        Tick(null, EventArgs.Empty);
        services.StartNotifications();
    }

    protected override void OnDisappearing()
    {
        timer?.Stop();
        base.OnDisappearing();
    }

    public async Task Pause()
    {
        timer?.Stop();
        await services.StopNotifications();
    }

    public void Resume()
    {
        timer?.Start();
        services.StartNotifications();
    }

    private void Tick(object? sender, EventArgs e) => clock.Text = ".NET MAUI \u00b7 " + DateTime.Now.ToString("M/d/yyyy, h:mm:ss tt", CultureInfo.GetCultureInfo("en-US"));
    private void Build()
    {
        header.Clear();
        var title = Ui.Text("CLASSIFIED ADS (.NET MAUI)", true);
        title.FontSize = 18;
        title.VerticalOptions = LayoutOptions.Center;
        var titleBar = new Grid
        {
            ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
            ColumnSpacing = 12
        };
        titleBar.Add(title);
        titleBar.Add(Ui.Button("Theme", "theme", Theme), 1);
        header.Add(titleBar);
        if (!services.SignedIn)
        {
            body.Content = new ScrollView
            {
                Content = Ui.Stack(Ui.Text("Welcome to Classified Ads", true), Ui.Text("Sign in to manage your workspace."), Ui.Button("Sign in", "lock", async () =>
                {
                    await services.Login();
                    Build();
                }), Ui.Button("Server settings", "configuration", Settings))
            };
            return;
        }

        var navigation = new HorizontalStackLayout
        {
            Spacing = 8
        };
        navigation.Add(Ui.Button("Home", "home", () =>
        {
            Home();
            return Task.CompletedTask;
        }));
        navigation.Add(Ui.Button("Products", "products", () => Navigate(new ProductsView(services))));
        navigation.Add(Ui.Button("Files", "files", () => Navigate(new FilesView(services))));
        navigation.Add(Ui.Button("Users", "users", () => Navigate(new UsersView(services))));
        navigation.Add(Ui.Button("Roles", "roles", () => Navigate(new RolesView(services))));
        navigation.Add(Ui.Button("Configuration Entries", "configuration", () => Navigate(new ConfigurationView(services))));
        navigation.Add(Ui.Button("Audit logs", "history", () => Navigate(new AuditLogsView(services))));
        navigation.Add(Ui.Button("Notifications", "notifications", () =>
        {
            Notifications();
            return Task.CompletedTask;
        }));
        header.Add(new ScrollView { Orientation = ScrollOrientation.Horizontal, Content = navigation });
        Home();
        services.StartNotifications();
    }

    private Task Navigate(View view)
    {
        body.Content = view;
        return Task.CompletedTask;
    }

    private void Home()
    {
        clock = Ui.Text("");
        Tick(null, EventArgs.Empty);
        var home = Ui.Stack();
        home.Spacing = 16;
        home.Add(Ui.Card(Ui.Stack(Ui.Text("Welcome to Classified Ads", true), clock, Ui.Text("Developed by Phong Nguyen \u00b7 @phongnguyend"), Ui.Button("Practical.CleanArchitecture", "link", async () => await Launcher.OpenAsync("https://github.com/phongnguyend/Practical.CleanArchitecture")))));
        home.Add(Ui.Button("Products", "products", () => Navigate(new ProductsView(services))));
        home.Add(Ui.Button("Files", "files", () => Navigate(new FilesView(services))));
        home.Add(Ui.Button("Users", "users", () => Navigate(new UsersView(services))));
        home.Add(Ui.Button("Roles", "roles", () => Navigate(new RolesView(services))));
        home.Add(Ui.Button("Configuration Entries", "configuration", () => Navigate(new ConfigurationView(services))));
        home.Add(Ui.Button("Sign out", "logout", async () =>
        {
            if (!await Dialog.Confirm("Sign out of Classified Ads?"))
            {
                return;
            }

            try
            {
                await services.Logout();
            }
            finally
            {
                Build();
            }
        }));
        body.Content = new ScrollView
        {
            Content = home
        };
    }

    private void Notifications()
    {
        status = Ui.Text(services.Status);
        messages = Ui.Stack();
        body.Content = new ScrollView
        {
            Content = Ui.Stack(Ui.Text("Notifications", true), status, messages)
        };
        UpdateNotifications();
    }

    private async void ShowToast(string message)
    {
        await toast.Show(message);
    }

    private void UpdateNotifications()
    {
        status.Text = services.Status;
        messages.Clear();
        foreach (var value in services.Messages)
        {
            messages.Add(Ui.Card(Ui.Text(value)));
        }

        if (services.Messages.Count == 0)
        {
            messages.Add(Ui.Text("No notifications yet"));
        }
    }

    private async Task Theme()
    {
        var dialog = new Dialog("Appearance");
        foreach (var (name, theme) in new[]
        {
            ("System", AppTheme.Unspecified),
            ("Light", AppTheme.Light),
            ("Dark", AppTheme.Dark)
        }

        )
        {
            dialog.Body.Add(Ui.Button(name, "theme", async () =>
            {
                Preferences.Set("theme", theme.ToString());
                Application.Current!.UserAppTheme = theme;
                await dialog.Close();
            }));
        }

        await dialog.Show();
    }

    private async Task Settings()
    {
        var current = services.Settings;
        var api = current.Api;
        var authority = current.Authority;
        var hub = current.Hub;
        var dialog = new Dialog("Server settings", () =>
        {
            foreach (var url in new[]
            {
                api,
                authority,
                hub
            }

            )
            {
                if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || uri.Scheme != "https")
                {
                    throw new InvalidOperationException("Use valid HTTPS server URLs.");
                }
            }

            Preferences.Set("api", api);
            Preferences.Set("authority", authority);
            Preferences.Set("hub", hub);
            services.NotificationsChanged -= UpdateNotifications;
            services.MessageReceived -= ShowToast;
            services = new AppServices();
            services.NotificationsChanged += UpdateNotifications;
            services.MessageReceived += ShowToast;
            return Task.FromResult(true);
        });
        Ui.Field(dialog.Body, "API URL", api, value => api = value);
        Ui.Field(dialog.Body, "Authority", authority, value => authority = value);
        Ui.Field(dialog.Body, "Notification hub", hub, value => hub = value);
        await dialog.Show();
    }
}
