using System.Globalization;
using ClassifiedAds.Mobile.Features.Notifications;
using ClassifiedAds.Mobile.Features.Products;
using ClassifiedAds.Mobile.Features.Files;
using ClassifiedAds.Mobile.Features.Users;
using ClassifiedAds.Mobile.Features.Roles;
using ClassifiedAds.Mobile.Features.ConfigurationEntries;
using ClassifiedAds.Mobile.Features.AuditLogs;
using ClassifiedAds.Mobile.Shared;

namespace ClassifiedAds.Mobile.Features.Workspace;

public sealed class WorkspacePage : ContentPage
{
    private AppServices services = new();
    private readonly ContentView body = new();
    private readonly Toast toast = new();
    private readonly VerticalStackLayout header = Ui.Stack();
    private readonly Dictionary<string, Button> navigationButtons = new();
    private Label clock = Ui.Text(""), status = Ui.Text("Disconnected");
    private VerticalStackLayout messages = Ui.Stack();
    private IDispatcherTimer? timer;
    private bool initialized;
    private string authenticationError = "";
    public WorkspacePage()
    {
        this.SetAppThemeColor(BackgroundColorProperty, Color.FromArgb("#f3f6fb"), Color.FromArgb("#0b1220"));
        var grid = new Grid
        {
            Padding = new Thickness(16, 8, 16, 16),
            RowSpacing = 12,
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
        services.Push.Opened += async () => await OpenPush();
        services.Push.Received += () => ShowToast("You have a new notification.");
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
                services.Push.Initialize();
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
        _ = SyncPush();
    }

    protected override void OnDisappearing()
    {
        timer?.Stop();
        base.OnDisappearing();
    }

    public async Task Pause()
    {
        timer?.Stop();
        if (services.LockSession())
        {
            toast.IsVisible = false;
            while (Navigation.ModalStack.LastOrDefault() is Dialog dialog)
            {
                await dialog.Close();
            }
            Build();
        }
        await services.StopNotifications();
    }

    public void Resume()
    {
        timer?.Start();
        services.StartNotifications();
        _ = SyncPush();
    }

    private async Task SyncPush()
    {
        try
        {
            await services.Push.Sync();
            await OpenPush();
        }
        catch
        {
            // Retry when the app resumes or notifications are enabled again.
        }
    }

    private async Task OpenPush()
    {
        var section = await services.Push.Consume();
        if (section != null && services.SignedIn)
        {
            await NavigatePush(section);
        }
    }

    private Task NavigatePush(string section)
    {
        return section switch
        {
            "products" => Navigate(new ProductsView(services)),
            "files" => Navigate(new FilesView(services)),
            "users" => Navigate(new UsersView(services)),
            "roles" => Navigate(new RolesView(services)),
            "configurationentries" => Navigate(new ConfigurationEntriesView(services)),
            "auditlogentries" => Navigate(new AuditLogsView(services)),
            _ => ShowPushInbox(),
        };
    }

    private Task ShowPushInbox()
    {
        Notifications();
        return Task.CompletedTask;
    }

    private void Tick(object? sender, EventArgs e) => clock.Text = ".NET MAUI \u00b7 " + DateTime.Now.ToString("M/d/yyyy, h:mm:ss tt", CultureInfo.GetCultureInfo("en-US"));
    private void Build()
    {
        header.Clear();
        navigationButtons.Clear();
        var title = Ui.Text("CLASSIFIED ADS (.NET MAUI)", true);
        title.FontSize = 16;
        title.VerticalOptions = LayoutOptions.Center;
        var titleBar = new Grid
        {
            ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
            ColumnSpacing = 12
        };
        titleBar.Add(title);
        var themeButton = Ui.Button("Theme", "theme", Theme);
        themeButton.Text = string.Empty;
        themeButton.WidthRequest = 48;
        themeButton.HeightRequest = 48;
        themeButton.Padding = 12;
        titleBar.Add(themeButton, 1);
        header.Add(titleBar);
        if (!string.IsNullOrEmpty(authenticationError))
        {
            var error = Ui.Text(authenticationError);
            error.FontSize = 14;
            var dismiss = Ui.Button("Dismiss error", "close", () =>
            {
                authenticationError = "";
                Build();
                return Task.CompletedTask;
            });
            dismiss.Text = string.Empty;
            dismiss.WidthRequest = 48;
            var errorRow = new Grid
            {
                ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
                ColumnSpacing = 8
            };
            errorRow.Add(error);
            errorRow.Add(dismiss, 1);
            header.Add(Ui.Card(errorRow));
        }
        if (!services.SignedIn)
        {
            var signedOut = Ui.Stack();
            if (services.BiometricEnabled)
            {
                var heading = Ui.Text("Workspace locked", true);
                heading.FontSize = 24;
                signedOut.Add(heading);
                signedOut.Add(Ui.Button("Unlock with biometrics", "lock", () => BiometricAction(services.UnlockSession)));
            }
            if (!services.BiometricEnabled)
            {
                var heading = Ui.Text("Your workspace, anywhere.", true);
                heading.FontSize = 24;
                signedOut.Add(heading);
            }
            signedOut.Add(Ui.Stack(Ui.Button("Sign in", "lock", async () =>
                {
                    await services.Login();
                    Build();
                }), Ui.Button("Server settings", "configuration", Settings)));
            body.Content = new ScrollView { Content = signedOut };
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
        navigation.Add(Ui.Button("Configuration Entries", "configuration", () => Navigate(new ConfigurationEntriesView(services))));
        navigation.Add(Ui.Button("Audit logs", "history", () => Navigate(new AuditLogsView(services))));
        navigation.Add(Ui.Button("Notifications", "notifications", () =>
        {
            Notifications();
            return Task.CompletedTask;
        }));
        navigation.Add(Ui.Button("Admin", "configuration", () => Navigate(new ClassifiedAds.Mobile.Features.Admin.AdminView(services))));
        foreach (var button in navigation.Children.OfType<Button>())
        {
            button.CornerRadius = 24;
            button.LineBreakMode = LineBreakMode.NoWrap;
            navigationButtons.Add(button.Text, button);
        }
        header.Add(new ScrollView { Orientation = ScrollOrientation.Horizontal, Content = navigation, HorizontalScrollBarVisibility = ScrollBarVisibility.Never });
        Home();
        services.StartNotifications();
        _ = SyncPush();
    }

    private async Task BiometricAction(Func<Task> action)
    {
        authenticationError = "";
        try
        {
            await action();
        }
        catch
        {
            authenticationError = "Biometric authentication did not complete. Try again or sign in with Identity Server.";
        }
        Build();
    }

    private Task Navigate(View view)
    {
        SelectNavigation(view switch
        {
            ProductsView => "Products",
            FilesView => "Files",
            UsersView => "Users",
            RolesView => "Roles",
            ConfigurationEntriesView => "Configuration Entries",
            AuditLogsView => "Audit logs",
            ClassifiedAds.Mobile.Features.Admin.AdminView => "Admin",
            _ => "Home"
        });
        body.Content = view;
        return Task.CompletedTask;
    }

    private void SelectNavigation(string title)
    {
        foreach (var (name, button) in navigationButtons)
        {
            var selected = name == title;
            button.BackgroundColor = Color.FromArgb(selected ? "#e0e7ff" : "#e2e8f0");
            button.BorderColor = selected ? Ui.Primary : Colors.Transparent;
            button.TextColor = selected ? Ui.Primary : Color.FromArgb("#334155");
            SemanticProperties.SetHint(button, selected ? "Current page" : "Open " + name);
        }
    }

    private void Home()
    {
        SelectNavigation("Home");
        clock = Ui.Text("");
        Tick(null, EventArgs.Empty);
        var home = Ui.Stack();
        home.Spacing = 12;
        var welcome = Ui.Text("Welcome to Classified Ads", true);
        welcome.FontSize = 16;
        var welcomeContent = Ui.Stack(welcome, clock, Ui.Text("Developed by Phong Nguyen \u00b7 @phongnguyend"),
            Ui.Button("Practical.CleanArchitecture", "link", async () => await Launcher.OpenAsync("https://github.com/phongnguyend/Practical.CleanArchitecture")));
        welcomeContent.Spacing = 6;
        home.Add(Ui.Card(welcomeContent));
        home.Add(Ui.Button("Products", "products", () => Navigate(new ProductsView(services))));
        home.Add(Ui.Button("Files", "files", () => Navigate(new FilesView(services))));
        home.Add(Ui.Button("Users", "users", () => Navigate(new UsersView(services))));
        home.Add(Ui.Button("Roles", "roles", () => Navigate(new RolesView(services))));
        home.Add(Ui.Button("Configuration Entries", "configuration", () => Navigate(new ConfigurationEntriesView(services))));
        home.Add(Ui.Button("Admin", "configuration", () => Navigate(new ClassifiedAds.Mobile.Features.Admin.AdminView(services))));
        if (services.BiometricAvailable || services.BiometricEnabled)
        {
            home.Add(Ui.Button(services.BiometricEnabled ? "Disable biometric login" : "Enable biometric login", "lock", () => BiometricAction(async () =>
            {
                if (services.BiometricEnabled)
                {
                    await services.DisableBiometrics();
                }
                else
                {
                    await services.EnableBiometrics();
                }
            })));
        }
        if (services.BiometricEnabled)
        {
            home.Add(Ui.Button("Lock app", "lock", async () =>
            {
                services.LockSession();
                await services.StopNotifications();
                Build();
            }));
        }
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
        SelectNavigation("Notifications");
        status = Ui.Text(services.Status);
        messages = Ui.Stack();
        body.Content = new ScrollView
        {
            Content = Ui.Stack(Ui.Text("Notifications", true), new PushInbox(services, NavigatePush), status, messages)
        };
        UpdateNotifications();
    }

    private async void ShowToast(string message)
    {
        if (services.SignedIn)
        {
            await toast.Show(message);
        }
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
