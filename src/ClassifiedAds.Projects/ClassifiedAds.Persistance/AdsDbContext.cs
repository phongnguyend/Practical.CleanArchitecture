using ClassifiedAds.Domain.Entities;
using ClassifiedAds.Persistance.MappingConfigurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ClassifiedAds.Persistance
{
    public class AdsDbContext : IdentityDbContext<User>
    {

        public AdsDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new UserConfiguration());
        }
    }
}
