using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClassifiedAds.Blazor
{
    public class LoginModel : PageModel
    {
        public async Task OnGetAsync()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                await HttpContext.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme);
            }
            else
            {
                Response.Redirect(Url.Content("~/").ToString());
            }
        }
    }
}