using System.Reflection;
using System.Text.Json;

namespace ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;

/// <summary>
/// https://stackoverflow.com/questions/78536/deep-cloning-objects
/// </summary>
public static class CloningExtensions
{
    public static T ShallowClone<T>(this T source)
    {
        var methodInfo = typeof(T).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);
        return (T)methodInfo.Invoke(source, null);
    }

    /// <summary>
    /// Perform a deep Copy of the object, using Json as a serialisation method. NOTE: Private members are not cloned using this method.
    /// </summary>
    /// <typeparam name="T">The type of object being copied.</typeparam>
    /// <param name="source">The object instance to copy.</param>
    /// <returns>The copied object.</returns>
    public static T DeepCloneJson<T>(this T source)
    {
        // Don't serialize a null object, simply return the default for that object
        if (ReferenceEquals(source, null))
        {
            return default(T);
        }

        return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(source));
    }
}
