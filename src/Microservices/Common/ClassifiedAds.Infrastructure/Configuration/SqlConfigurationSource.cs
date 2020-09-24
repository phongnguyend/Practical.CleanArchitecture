using Microsoft.Extensions.Configuration;

namespace ClassifiedAds.Infrastructure.Configuration
{
    public class SqlConfigurationSource : IConfigurationSource
    {
        private readonly string _connectionString;

        public SqlConfigurationSource(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new SqlConfigurationProvider(_connectionString);
        }
    }
}
