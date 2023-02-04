using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(ClassifiedAds.IdentityServer.Areas.Identity.IdentityHostingStartup))]
namespace ClassifiedAds.IdentityServer.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
        }
    }
}