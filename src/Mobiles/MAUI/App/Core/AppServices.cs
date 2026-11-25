using ClassifiedAds.Mobile.Core.Network;
using Duende.IdentityModel.OidcClient;
using Duende.IdentityModel.OidcClient.Browser;
using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;

namespace ClassifiedAds.Mobile;

public sealed record ServerSettings(string Api, string Authority, string Hub)
{
    public static ServerSettings Load() => new(Preferences.Get("api", "https://localhost:44312/api/"), Preferences.Get("authority", "https://localhost:44367"), Preferences.Get("hub", "https://localhost:44312/hubs/notification"));
}

public sealed class SystemBrowser : Duende.IdentityModel.OidcClient.Browser.IBrowser
{
    public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await WebAuthenticator.Default.AuthenticateAsync(new Uri(options.StartUrl), new Uri(options.EndUrl));
            return new BrowserResult
            {
                ResultType = BrowserResultType.Success,
                Response = options.EndUrl + "?" + string.Join("&", result.Properties.Select(p => Uri.EscapeDataString(p.Key) + "=" + Uri.EscapeDataString(p.Value)))
            };
        }
        catch (TaskCanceledException)
        {
            return new BrowserResult
            {
                ResultType = BrowserResultType.UserCancel
            };
        }
    }
}

public sealed record StoredSession(string AccessToken, string RefreshToken, string IdentityToken, DateTimeOffset ExpiresAt);
public sealed class AppServices
{
    private const string SessionKey = "classifiedads.maui.session";
    private readonly SemaphoreSlim sessionLock = new(1, 1);
    private readonly OidcClient auth;
    private StoredSession? session;
    private int generation;
    public bool BiometricEnabled => BiometricVault.Enabled;

    public bool BiometricAvailable => BiometricVault.IsAvailable();

    public bool LockSession()
    {
        Interlocked.Increment(ref generation);
        if (!BiometricEnabled)
        {
            return false;
        }

        session = null;
        Messages.Clear();
        return true;
    }

    public async Task EnableBiometrics()
    {
        var current = session ?? throw new InvalidOperationException("Sign in first.");
        if (string.IsNullOrEmpty(current.RefreshToken))
        {
            throw new InvalidOperationException("Sign in again to enable biometric login.");
        }

        var epoch = generation;
        var data = JsonSerializer.Serialize(current);
        await BiometricVault.Create();
        await BiometricVault.Write(data);
        if (await BiometricVault.Read() != data || epoch != generation)
        {
            throw new InvalidOperationException("Biometric enrollment did not complete.");
        }

        BiometricVault.Enabled = true;
        SecureStorage.Remove(SessionKey);
    }

    public async Task DisableBiometrics()
    {
        var current = session ?? throw new InvalidOperationException("Unlock before changing biometric settings.");
        var epoch = generation;
        await BiometricVault.Read();
        if (epoch != generation)
        {
            throw new InvalidOperationException("Session changed. Unlock again.");
        }

        await SecureStorage.SetAsync(SessionKey, JsonSerializer.Serialize(current));
        BiometricVault.Clear();
    }

    public async Task UnlockSession()
    {
        if (!BiometricEnabled)
        {
            throw new InvalidOperationException("Biometric login is not enabled.");
        }

        var epoch = generation;
        var data = await BiometricVault.Read();
        if (epoch != generation)
        {
            throw new InvalidOperationException("Session changed. Unlock again.");
        }

        session = JsonSerializer.Deserialize<StoredSession>(data);
        try
        {
            await AccessToken();
        }
        catch
        {
            session = null;
            throw;
        }
    }
    private HubConnection? hub;
    private CancellationTokenSource? hubLifetime;
    public ApiClient Api { get; }

    public PushNotifications Push { get; }

    public ServerSettings Settings { get; } = ServerSettings.Load();

    public bool SignedIn => session is not null;
    public string Status { get; private set; } = "Disconnected";

    public List<string> Messages { get; } = [];


    public event Action? NotificationsChanged;
    public event Action<string>? MessageReceived;
    public AppServices()
    {
        Push = new PushNotifications(this);
        auth = new OidcClient(new OidcClientOptions { Authority = Settings.Authority, ClientId = "ClassifiedAds.Mobile.MAUI", RedirectUri = "classifiedads.maui:/oauthredirect", PostLogoutRedirectUri = "classifiedads.maui:/logout", Scope = "openid profile offline_access ClassifiedAds.WebAPI", Browser = new SystemBrowser() });
        Api = new ApiClient(new HttpClient { BaseAddress = new Uri(Settings.Api.TrimEnd('/') + "/"), Timeout = TimeSpan.FromSeconds(60) }, AccessToken);
    }

    private async Task DisablePushBeforeSessionChange()
    {
        try
        {
            await Push.Disable();
        }
        catch
        {
            // Native token cleanup runs even if unregistering with the API fails.
        }
    }

    public async Task Restore()
    {
        session = null;
        if (BiometricEnabled)
        {
            return;
        }

        var stored = await SecureStorage.GetAsync(SessionKey);
        if (stored is not null)
        {
            try
            {
                session = JsonSerializer.Deserialize<StoredSession>(stored);
            }
            catch (JsonException)
            {
                SecureStorage.Remove(SessionKey);
            }
        }
    }

    public async Task Login()
    {
        await DisablePushBeforeSessionChange();
        BiometricVault.Clear();
        SecureStorage.Remove(SessionKey);
        session = null;
        var result = await auth.LoginAsync(new LoginRequest());
        if (result.IsError)
        {
            throw new InvalidOperationException(result.ErrorDescription ?? result.Error);
        }

        await Save(new(result.AccessToken, result.RefreshToken, result.IdentityToken, result.AccessTokenExpiration));
    }

    private async Task Save(StoredSession value)
    {
        var epoch = generation;
        if (BiometricEnabled)
        {
            await BiometricVault.Write(JsonSerializer.Serialize(value));
        }
        else
        {
            await SecureStorage.SetAsync(SessionKey, JsonSerializer.Serialize(value));
        }

        if (epoch != generation)
        {
            throw new InvalidOperationException("Session locked. Unlock again.");
        }

        session = value;
    }

    public async Task<string> AccessToken()
    {
        await sessionLock.WaitAsync();
        try
        {
            if (session is null)
            {
                throw new InvalidOperationException("Please sign in.");
            }

            if (session.ExpiresAt > DateTimeOffset.UtcNow.AddSeconds(60))
            {
                return session.AccessToken;
            }

            if (string.IsNullOrEmpty(session.RefreshToken))
            {
                throw new InvalidOperationException("Session expired. Please sign out and sign in again.");
            }

            var previous = session;
            var result = await auth.RefreshTokenAsync(previous.RefreshToken);
            if (!ReferenceEquals(session, previous))
            {
                throw new InvalidOperationException("Session locked. Unlock again.");
            }
            if (result.IsError)
            {
                throw new InvalidOperationException("Session expired. Please sign out and sign in again.");
            }

            await Save(new(result.AccessToken, string.IsNullOrEmpty(result.RefreshToken) ? session.RefreshToken : result.RefreshToken, string.IsNullOrEmpty(result.IdentityToken) ? session.IdentityToken : result.IdentityToken, result.AccessTokenExpiration));
            return session.AccessToken;
        }
        finally
        {
            sessionLock.Release();
        }
    }

    public async Task Logout()
    {
        await DisablePushBeforeSessionChange();
        await StopNotifications();
        await sessionLock.WaitAsync();
        string? id;
        try
        {
            id = session?.IdentityToken;
            session = null;
            SecureStorage.Remove(SessionKey);
            BiometricVault.Clear();
            Interlocked.Increment(ref generation);
            Messages.Clear();
        }
        finally
        {
            sessionLock.Release();
        }

        if (!string.IsNullOrEmpty(id))
        {
            await auth.LogoutAsync(new LogoutRequest { IdTokenHint = id });
        }
    }

    private void Publish(string status)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Status = status;
            NotificationsChanged?.Invoke();
        });
    }

    public void StartNotifications()
    {
        if (hubLifetime is not null || !SignedIn)
        {
            return;
        }

        hubLifetime = new CancellationTokenSource();
        hub = new HubConnectionBuilder().WithUrl(Settings.Hub, o => o.AccessTokenProvider = async () => await AccessToken()).WithAutomaticReconnect().Build();
        hub.On<JsonElement>("ReceiveMessage", value => MainThread.BeginInvokeOnMainThread(() =>
        {
            Messages.Insert(0, value.ValueKind == JsonValueKind.String ? value.GetString()! : value.GetRawText());
            MessageReceived?.Invoke(Messages[0]);
            if (Messages.Count > 100)
            {
                Messages.RemoveRange(100, Messages.Count - 100);
            }

            NotificationsChanged?.Invoke();
        }));
        hub.Reconnecting += _ =>
        {
            Publish("Reconnecting...");
            return Task.CompletedTask;
        };
        hub.Reconnected += _ =>
        {
            Publish("Connected to NotificationHub");
            return Task.CompletedTask;
        };
        var connection = hub;
        var token = hubLifetime.Token;
        hub.Closed += _ => Connect(connection, token);
        _ = Connect(connection, token);
    }

    private async Task Connect(HubConnection connection, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                await connection.StartAsync(token);
                Publish("Connected to NotificationHub");
                return;
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch
            {
                Publish("Cannot connect to NotificationHub. Retrying...");
            }

            try
            {
                await Task.Delay(10000, token);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
    }

    public async Task StopNotifications()
    {
        hubLifetime?.Cancel();
        if (hub is not null)
        {
            await hub.DisposeAsync();
        }

        hubLifetime?.Dispose();
        hubLifetime = null;
        hub = null;
        Publish("Disconnected");
    }

    public async Task Download(string path, string fileName)
    {
        using var response = await Api.Send(path, HttpMethod.Get);
        var folder = Path.Combine(FileSystem.CacheDirectory, "exports");
        Directory.CreateDirectory(folder);
        foreach (var old in Directory.GetFiles(folder))
        {
            if (File.GetLastWriteTimeUtc(old) < DateTime.UtcNow.AddDays(-1))
            {
                File.Delete(old);
            }
        }

        var file = Path.Combine(folder, Path.GetFileName(fileName));
        await using (var output = File.Create(file))
        {
            await response.Content.CopyToAsync(output);
        }

        await Share.Default.RequestAsync(new ShareFileRequest("Share file", new ShareFile(file)));
    }
}
