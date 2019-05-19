using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Persistence.MappingConfigurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ClassifiedAds.Persistence
{
    public class AdsDbContext : IdentityDbContext<User>
    {

        public AdsDbContext(DbContextOptions<AdsDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new ProductConfiguration());
        }
    }
}
