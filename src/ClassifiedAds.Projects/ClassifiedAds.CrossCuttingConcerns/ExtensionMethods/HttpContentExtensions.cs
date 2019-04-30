using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ClassifiedAds.CrossCuttingConcerns.ExtensionMethods
{
    public static class HttpContentExtensions
    {
        public static async Task<T> ReadAs<T>(this HttpContent httpContent)
        {
            using (var streamReader = new StreamReader(await httpContent.ReadAsStreamAsync()))
            {
                using (var jsonTextReader = new JsonTextReader(streamReader))
                {
                    var jsonSerializer = new JsonSerializer();
                    return jsonSerializer.Deserialize<T>(jsonTextReader);
                }
            }
        }

        public static StringContent AsStringContent(this object obj, string contentType)
        {
            var content = new StringContent(JsonConvert.SerializeObject(obj));
            content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            return content;
        }

        public static StringContent AsJsonContent(this object obj)
        {
            return obj.AsStringContent("application/json");
        }
    }
}
