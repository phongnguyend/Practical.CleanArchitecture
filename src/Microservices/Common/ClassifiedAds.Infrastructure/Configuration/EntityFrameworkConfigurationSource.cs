using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace ClassifiedAds.Infrastructure.Configuration
{
    public class EntityFrameworkConfigurationSource : IConfigurationSource
    {
        private readonly Func<DbContext> _dbContextResolver;

        public EntityFrameworkConfigurationSource(Func<DbContext> dbContextResolver)
        {
            _dbContextResolver = dbContextResolver;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new EntityFrameworkConfigurationProvider(_dbContextResolver);
        }
    }
}
