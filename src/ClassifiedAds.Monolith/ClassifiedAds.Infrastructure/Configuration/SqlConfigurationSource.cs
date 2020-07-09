using Microsoft.Extensions.Configuration;

namespace ClassifiedAds.Infrastructure.Configuration
{
    public class SqlConfigurationSource : IConfigurationSource
    {
        private readonly SqlServerSourceOptions _options;

        public SqlConfigurationSource(SqlServerSourceOptions options)
        {
            _options = options;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new SqlConfigurationProvider(_options);
        }
    }
}
