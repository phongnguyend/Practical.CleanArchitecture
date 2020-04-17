using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace ClassifiedAds.Infrastructure.Configuration
{
    public static class EntityFrameworkExtensions
    {
        public static IConfigurationBuilder AddEFConfiguration(this IConfigurationBuilder builder,
            Func<DbContext> dbContextResolver)
        {
            return builder.Add(new EntityFrameworkConfigurationSource(dbContextResolver));
        }
    }
}
