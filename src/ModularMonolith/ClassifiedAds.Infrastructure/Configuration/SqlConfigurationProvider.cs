using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace ClassifiedAds.Infrastructure.Configuration
{
    public class SqlConfigurationProvider : ConfigurationProvider
    {
        private readonly SqlServerOptions _options;

        public SqlConfigurationProvider(SqlServerOptions options)
        {
            _options = options;
        }

        public override void Load()
        {
            using (var conn = new SqlConnection(_options.ConnectionString))
            {
                conn.Open();
                Data = conn.Query<ConfigurationEntry>(_options.SqlQuery).ToDictionary(c => c.Key, c => c.Value);
            }
        }
    }

    public class ConfigurationEntry
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
