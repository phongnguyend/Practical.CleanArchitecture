using ClassifiedAds.Mobile.Core.Models;
using System.Text.Json;

namespace ClassifiedAds.Mobile.Core.Network;

public sealed class Repository<T>(ApiClient api, string resource)
    where T : Record, new()
{
    public Task<List<T>> List() => api.Get<List<T>>(resource);
    public Task<T> Get(string id) => api.Get<T>(ApiClient.PathFor(resource, id));
    public Task Delete(string id) => api.Write(ApiClient.PathFor(resource, id), HttpMethod.Delete);
    public Task Save(T value) => api.Write(string.IsNullOrEmpty(value.Id) ? resource : ApiClient.PathFor(resource, value.Id), string.IsNullOrEmpty(value.Id) ? HttpMethod.Post : HttpMethod.Put, value.Payload());
    public Task<JsonElement> History(string id) => api.Get<JsonElement>(ApiClient.PathFor(resource, id) + "/auditlogs");
}
