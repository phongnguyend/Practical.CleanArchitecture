using ClassifiedAds.Domain.Entities;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace ClassifiedAds.Infrastructure.Configuration
{
    public class SqlConfigurationProvider : ConfigurationProvider
    {
        private readonly string _connectionString;

        public SqlConfigurationProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public override void Load()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                Data = conn.Query<ConfigurationEntry>("select * from ConfigurationEntries").ToDictionary(c => c.Key, c => c.Value);
            }
        }
    }
}
