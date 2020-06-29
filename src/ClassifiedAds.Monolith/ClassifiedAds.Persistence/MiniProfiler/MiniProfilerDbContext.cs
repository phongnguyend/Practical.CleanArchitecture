using Microsoft.EntityFrameworkCore;

namespace ClassifiedAds.Persistence.MiniProfiler
{
    public class MiniProfilerDbContext : DbContext
    {
        public MiniProfilerDbContext(DbContextOptions<MiniProfilerDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MiniProfilers>().HasIndex(x => x.Id).IsUnique();
            modelBuilder.Entity<MiniProfilers>().HasIndex(x => new { x.User, x.HasUserViewed }).IncludeProperties(x => new { x.Id, x.Started });

            modelBuilder.Entity<MiniProfilerTimings>().HasIndex(x => x.Id).IsUnique();
            modelBuilder.Entity<MiniProfilerTimings>().HasIndex(x => x.MiniProfilerId);

            modelBuilder.Entity<MiniProfilerClientTimings>().HasIndex(x => x.Id).IsUnique();
            modelBuilder.Entity<MiniProfilerClientTimings>().HasIndex(x => x.MiniProfilerId);
        }
    }
}
