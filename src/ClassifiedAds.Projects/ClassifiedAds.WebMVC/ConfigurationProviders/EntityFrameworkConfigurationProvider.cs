using ClassifiedAds.DomainServices.Entities;
using ClassifiedAds.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace ClassifiedAds.WebMVC.ConfigurationProviders
{
    public class EntityFrameworkConfigurationProvider : ConfigurationProvider
    {
        public EntityFrameworkConfigurationProvider(Action<DbContextOptionsBuilder> optionsAction)
        {
            OptionsAction = optionsAction;
        }

        public Action<DbContextOptionsBuilder> OptionsAction { get; }

        public override void Load()
        {
            var builder = new DbContextOptionsBuilder<AdsDbContext>();

            OptionsAction(builder);

            using (var dbContext = new AdsDbContext(builder.Options))
            {
                Data = dbContext.Set<ConfigurationEntry>().ToDictionary(c => c.Key, c => c.Value);
            }
        }
    }
}
