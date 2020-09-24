using ClassifiedAds.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace ClassifiedAds.Infrastructure.Configuration
{
    public class EntityFrameworkConfigurationProvider : ConfigurationProvider
    {
        private readonly Func<DbContext> _dbContextResolver;

        public EntityFrameworkConfigurationProvider(Func<DbContext> dbContextResolver)
        {
            _dbContextResolver = dbContextResolver;
        }

        public override void Load()
        {
            using (var dbct = _dbContextResolver.Invoke())
            {
                Data = dbct.Set<ConfigurationEntry>().ToDictionary(c => c.Key, c => c.Value);
            }
        }
    }
}
