using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;

public static class ObjectExtensions
{
    public static string AsJsonString(this object obj)
    {
        var content = JsonSerializer.Serialize(obj, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
        });
        return content;
    }
}
