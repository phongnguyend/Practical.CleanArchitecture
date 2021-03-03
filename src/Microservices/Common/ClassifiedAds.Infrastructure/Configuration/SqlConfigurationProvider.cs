using CryptographyHelper;
using CryptographyHelper.AsymmetricAlgorithms;
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
                var data = conn.Query<ConfigurationEntry>(_options.SqlQuery).ToList();

                var cert = data.Any(x => x.IsSensitive)
                    ? _options.Certificate.FindCertificate()
                    : null;

                foreach (var entry in data)
                {
                    if (entry.IsSensitive)
                    {
                        var decrypted = entry.Value.FromBase64String().UseRSA(cert).Decrypt();
                        entry.Value = decrypted.GetString();
                    }
                }

                Data = data.ToDictionary(c => c.Key, c => c.Value);
            }
        }
    }

    public class ConfigurationEntry
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public bool IsSensitive { get; set; }
    }
}
