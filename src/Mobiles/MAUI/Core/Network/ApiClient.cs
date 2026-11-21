using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace ClassifiedAds.Mobile.Core.Network;

public sealed class ApiClient(HttpClient http, Func<Task<string>> token)
{
    public static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };
    public async Task<HttpResponseMessage> Send(string path, HttpMethod method, HttpContent? body = null)
    {
        using var request = new HttpRequestMessage(method, path)
        {
            Content = body
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await token());
        var response = await http.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var message = await response.Content.ReadAsStringAsync();
            var status = (int)response.StatusCode;
            response.Dispose();
            throw new HttpRequestException($"Request failed ({status}): {message}");
        }

        return response;
    }

    public async Task<T> Get<T>(string path)
    {
        using var response = await Send(path, HttpMethod.Get);
        return (await response.Content.ReadFromJsonAsync<T>(Json))!;
    }

    public async Task Write(string path, HttpMethod method, object? value = null)
    {
        using var response = await Send(path, method, value is null ? null : JsonContent.Create(value, options: Json));
    }

    public async Task<string> Upload(string path, Stream stream, string name, IDictionary<string, string>? fields = null)
    {
        using var body = new MultipartFormDataContent();
        var file = new StreamContent(stream);
        file.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        body.Add(file, "formFile", Path.GetFileName(name));
        foreach (var pair in fields ?? new Dictionary<string, string>())
        {
            body.Add(new StringContent(pair.Value), pair.Key);
        }

        using var response = await Send(path, HttpMethod.Post, body);
        return await response.Content.ReadAsStringAsync();
    }

    public static string PathFor(string resource, string id) => resource + "/" + Uri.EscapeDataString(id);
}
