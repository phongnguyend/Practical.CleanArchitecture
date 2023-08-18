using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServerHost.Pages.ExternalLogin;

[AllowAnonymous]
[SecurityHeaders]
public class Challenge : PageModel
{
    private readonly IIdentityServerInteractionService _interactionService;

    public Challenge(IIdentityServerInteractionService interactionService)
    {
        _interactionService = interactionService;
    }
        
    public IActionResult OnGet(string scheme, string returnUrl)
    {
        if (string.IsNullOrEmpty(returnUrl)) returnUrl = "~/";

        // validate returnUrl - either it is a valid OIDC URL or back to a local page
        if (Url.IsLocalUrl(returnUrl) == false && _interactionService.IsValidReturnUrl(returnUrl) == false)
        {
            // user might have clicked on a malicious link - should be logged
            throw new Exception("invalid return URL");
        }
            
        // start challenge and roundtrip the return URL and scheme 
        var props = new AuthenticationProperties
        {
            RedirectUri = Url.Page("/externallogin/callback"),
                
            Items =
            {
                { "returnUrl", returnUrl }, 
                { "scheme", scheme },
            }
        };

        return Challenge(props, scheme);
    }
}