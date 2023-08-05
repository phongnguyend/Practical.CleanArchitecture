using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace ReverseProxy.Yarp.Controllers
{
    public class AuthenticationController : ControllerBase
    {
        private readonly IAntiforgery _forgeryService;

        public AuthenticationController(IAntiforgery forgeryService)
        {
            _forgeryService = forgeryService;
        }

        [HttpGet("/login")]
        public async Task LoginAsync(string returnUrl)
        {
            if (HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                Response.Redirect(Url.Content("~/").ToString());
            }
            else
            {
                await HttpContext.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties
                {
                    RedirectUri = Url.IsLocalUrl(returnUrl) ? returnUrl : "/"
                });
            }
        }

        [HttpGet("/logout")]
        public async Task LogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpGet("/userinfor")]
        public IActionResult UserInfor()
        {
            if (HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                var tokens = _forgeryService.GetAndStoreTokens(HttpContext);
                HttpContext.Response.Cookies.Append("PHONG-XSRF-TOKEN", tokens.RequestToken!, new CookieOptions { HttpOnly = false });

                return Ok(new
                {
                    Id = "",
                    FirstName = "Phong",
                    LastName = "Nguyen",
                    Timestamp = DateTimeOffset.Now
                });
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
