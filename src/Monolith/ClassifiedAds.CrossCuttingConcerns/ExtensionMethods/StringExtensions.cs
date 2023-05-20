using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;

public static class StringExtensions
{
    public static T ParseTo<T>(this string s, IFormatProvider provider)
        where T : IParsable<T>
    {
        return T.Parse(s, provider);
    }

    public static bool TryParseTo<T>(this string s, IFormatProvider provider, out T result)
        where T : IParsable<T>
    {
        return T.TryParse(s, provider, out result);
    }

    public static string Left(this string value, int length)
    {
        length = Math.Abs(length);
        return string.IsNullOrEmpty(value) ? value : value.Substring(0, Math.Min(value.Length, length));
    }

    public static string Right(this string value, int length)
    {
        length = Math.Abs(length);
        return string.IsNullOrEmpty(value) ? value : value.Substring(value.Length - Math.Min(value.Length, length));
    }

    public static bool In(this string value, List<string> list)
    {
        return list.Contains(value, StringComparer.OrdinalIgnoreCase);
    }

    public static bool NotIn(this string value, List<string> list)
    {
        return !In(value, list);
    }

    public static bool EqualsIgnoreCase(this string source, string toCheck)
    {
        return string.Equals(source, toCheck, StringComparison.OrdinalIgnoreCase);
    }

    public static string ToBase64(this string src)
    {
        byte[] b = Encoding.UTF8.GetBytes(src);
        return Convert.ToBase64String(b);
    }

    public static string ToBase64(this string src, Encoding encoding)
    {
        byte[] b = encoding.GetBytes(src);
        return Convert.ToBase64String(b);
    }

    public static string FromBase64String(this string src)
    {
        byte[] b = Convert.FromBase64String(src);
        return Encoding.UTF8.GetString(b);
    }

    public static string FromBase64String(this string src, Encoding encoding)
    {
        byte[] b = Convert.FromBase64String(src);
        return encoding.GetString(b);
    }

    public static string Remove(this string source, params string[] removedValues)
    {
        removedValues.ToList().ForEach(x => source = source.Replace(x, string.Empty));
        return source;
    }

    public static string ToUpperInvariantCulture(this string source)
    {
        return source.ToUpper(CultureInfo.InvariantCulture);
    }
}
