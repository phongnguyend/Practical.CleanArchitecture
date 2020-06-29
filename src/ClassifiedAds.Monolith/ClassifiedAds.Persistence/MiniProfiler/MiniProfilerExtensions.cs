using ClassifiedAds.Persistence.MiniProfiler;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MiniProfilerExtensions
    {
        public static void MigrateMiniProfilerDb(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<MiniProfilerDbContext>().Database.Migrate();
            }
        }
    }
}
