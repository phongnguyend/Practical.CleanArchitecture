using Microsoft.Extensions.Configuration;

namespace ClassifiedAds.Infrastructure.Configuration
{
    public class SqlConfigurationSource : IConfigurationSource
    {
        private readonly SqlServerOptions _options;

        public SqlConfigurationSource(SqlServerOptions options)
        {
            _options = options;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new SqlConfigurationProvider(_options);
        }
    }
}
