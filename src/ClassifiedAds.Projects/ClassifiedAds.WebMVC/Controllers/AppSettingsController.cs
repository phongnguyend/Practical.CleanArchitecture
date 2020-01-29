using ClassifiedAds.WebMVC.ConfigurationOptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ClassifiedAds.WebMVC.Controllers
{
    public class AppSettingsController : Controller
    {
        private readonly AppSettings _appSettings;

        public AppSettingsController(IOptionsSnapshot<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public IActionResult Index()
        {
            string text = @"
            const appSettings = {
                NotificationServer: {
                    Endpoint: '" + _appSettings.NotificationServer.PublicEndpoint + @"'
                }
            }";

            var rs = Content(text);
            rs.ContentType = "application/javascript";
            return rs;
        }
    }
}
