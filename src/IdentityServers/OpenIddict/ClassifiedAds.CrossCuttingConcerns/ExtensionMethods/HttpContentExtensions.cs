using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;

public static class HttpContentExtensions
{
    public static async Task<T> ReadAs<T>(this HttpContent httpContent)
    {
        var json = await httpContent.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(json))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        });
    }

    public static StringContent AsStringContent(this object obj, string contentType)
    {
        var content = new StringContent(JsonSerializer.Serialize(obj));
        content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        return content;
    }

    public static StringContent AsJsonContent(this object obj)
    {
        return obj.AsStringContent("application/json");
    }
}
