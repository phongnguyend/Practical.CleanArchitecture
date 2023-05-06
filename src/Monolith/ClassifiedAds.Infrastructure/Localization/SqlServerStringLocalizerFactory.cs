using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassifiedAds.Infrastructure.Localization;

public class SqlServerStringLocalizerFactory : IStringLocalizerFactory
{
    private readonly SqlServerOptions _options;
    private readonly IMemoryCache _memoryCache;

    public SqlServerStringLocalizerFactory(IOptions<SqlServerOptions> options, IMemoryCache memoryCache)
    {
        _options = options.Value;
        _memoryCache = memoryCache;
    }

    public IStringLocalizer Create(Type resourceSource)
    {
        return new SqlServerStringLocalizer(LoadData());
    }

    public IStringLocalizer Create(string baseName, string location)
    {
        return new SqlServerStringLocalizer(LoadData());
    }

    private Dictionary<string, Dictionary<string, string>> LoadData()
    {
        var data = _memoryCache.Get<Dictionary<string, Dictionary<string, string>>>(typeof(SqlServerStringLocalizerFactory).FullName);

        if (data == null)
        {
            using var conn = new SqlConnection(_options.ConnectionString);
            {
                conn.Open();
                data = conn.Query<LocalizationEntry>(_options.SqlQuery)
                    .GroupBy(x => x.Name)
                    .ToDictionary(x => x.Key, x => x.ToDictionary(y => y.Culture, y => y.Value));
            }

            _memoryCache.Set(typeof(SqlServerStringLocalizerFactory).FullName, data, DateTimeOffset.Now.AddMinutes(_options.CacheMinutes));
        }

        return data;
    }
}

public class LocalizationEntry
{
    public string Name { get; set; }

    public string Value { get; set; }

    public string Culture { get; set; }
}
