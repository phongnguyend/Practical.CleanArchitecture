using Microsoft.Extensions.Configuration;

namespace ClassifiedAds.Infrastructure.Configuration
{
    public static class SqlConfigurationExtensions
    {
        /// <summary>
        ///     Adds an Microsoft.Extensions.Configuration.IConfigurationProvider that reads
        ///    configuration values from a custom table in the Sql Server Database.
        /// </summary>
        /// <param name="builder">The Microsoft.Extensions.Configuration.IConfigurationBuilder to add to.</param>
        /// <param name="connectionString">Connection String to the database.</param>
        /// <returns>The Microsoft.Extensions.Configuration.IConfigurationBuilder.</returns>
        public static IConfigurationBuilder AddSqlConfigurationVariables(this IConfigurationBuilder builder, string connectionString)
        {
            return builder.Add(new SqlConfigurationSource(connectionString));
        }
    }
}
