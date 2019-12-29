using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ClassifiedAds.Persistence
{
    public class AdsDbContext : DbContext
    {
        public AdsDbContext(DbContextOptions<AdsDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
