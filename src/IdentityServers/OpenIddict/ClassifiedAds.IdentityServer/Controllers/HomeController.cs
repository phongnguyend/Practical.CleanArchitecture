using Microsoft.AspNetCore.Mvc;

namespace ClassifiedAds.IdentityServer.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
