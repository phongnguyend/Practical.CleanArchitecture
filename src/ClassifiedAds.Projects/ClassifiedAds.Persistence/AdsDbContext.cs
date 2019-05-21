using ClassifiedAds.Persistence.MappingConfigurations;
using Microsoft.EntityFrameworkCore;

namespace ClassifiedAds.Persistence
{
    public class AdsDbContext : DbContext
    {

        public AdsDbContext(DbContextOptions<AdsDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new UserTokenConfiguration());
            builder.ApplyConfiguration(new RoleConfiguration());
            builder.ApplyConfiguration(new ProductConfiguration());
            builder.ApplyConfiguration(new SmsMessageConfiguration());
            builder.ApplyConfiguration(new EmailMessageConfiguration());
        }
    }
}
