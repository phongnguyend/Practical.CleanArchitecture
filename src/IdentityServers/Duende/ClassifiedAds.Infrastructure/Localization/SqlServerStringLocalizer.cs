using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ClassifiedAds.Infrastructure.Localization;

public class SqlServerStringLocalizer : IStringLocalizer
{
    private readonly Dictionary<string, Dictionary<string, string>> _data;

    public SqlServerStringLocalizer(Dictionary<string, Dictionary<string, string>> data)
    {
        _data = data;
    }

    public LocalizedString this[string name]
    {
        get
        {
            var value = GetString(name);
            return new LocalizedString(name, value);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var format = GetString(name);
            var value = string.Format(format ?? name, arguments);
            return new LocalizedString(name, value);
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        throw new NotImplementedException();
    }

    private string GetString(string name)
    {
        var culture = CultureInfo.CurrentCulture.ToString();

        if (_data.ContainsKey(name) && _data[name].ContainsKey(culture))
        {
            return _data[name][culture];
        }

        return name;
    }
}
