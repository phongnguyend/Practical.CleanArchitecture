using ClassifiedAds.Blazor.Modules.Core.Services;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClassifiedAds.Blazor.Modules.Admin.Services;

public class NotificationPlaygroundService : HttpService
{
    public NotificationPlaygroundService(HttpClient client, ITokenManager tokenManager)
        : base(client, tokenManager)
    {
    }

    public Task<JsonElement> LoadAsync(string channel)
    {
        return GetAsync<JsonElement>(Path(channel));
    }

    public Task<JsonElement> QueueAsync(string channel, object message)
    {
        return PostAsync<JsonElement>(Path(channel), message);
    }

    private static string Path(string channel)
    {
        return channel == "mobile-push" ? "api/notifications/playground" : $"api/notifications/playgrounds/{channel}";
    }
}
