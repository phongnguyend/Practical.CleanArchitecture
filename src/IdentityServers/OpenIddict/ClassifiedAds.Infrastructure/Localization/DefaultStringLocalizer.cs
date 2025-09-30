using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;

namespace ClassifiedAds.Infrastructure.Localization;

public class DefaultStringLocalizer : IStringLocalizer
{
    public LocalizedString this[string name]
    {
        get
        {
            return new LocalizedString(name, name);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var value = string.Format(name, arguments);
            return new LocalizedString(name, value);
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        throw new NotImplementedException();
    }
}
