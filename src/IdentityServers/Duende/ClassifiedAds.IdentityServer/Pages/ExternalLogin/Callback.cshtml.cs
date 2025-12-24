using ClassifiedAds.Domain.Entities;
using Duende.IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace IdentityServerHost.Pages.ExternalLogin;

[AllowAnonymous]
[SecurityHeaders]
public class Callback : PageModel
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IIdentityServerInteractionService _interaction;
    private readonly ILogger<Callback> _logger;
    private readonly IEventService _events;

    public Callback(
        IIdentityServerInteractionService interaction,
        IEventService events,
        ILogger<Callback> logger,
        UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _interaction = interaction;
        _logger = logger;
        _events = events;
    }

    public async Task<IActionResult> OnGet()
    {
        // read external identity from the temporary cookie
        var result = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
        if (result?.Succeeded != true)
        {
            throw new Exception("External authentication error");
        }

        var externalUser = result.Principal;

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            var externalClaims = externalUser.Claims.Select(c => $"{c.Type}: {c.Value}");
            _logger.LogDebug("External claims: {@claims}", externalClaims);
        }

        // lookup our user and external provider info
        // try to determine the unique id of the external user (issued by the provider)
        // the most common claim type for that are the sub claim and the NameIdentifier
        // depending on the external provider, some other claim type might be used

        var provider = result.Properties.Items["scheme"];

        Claim userIdClaim = null;

        if (provider == "AAD")
        {
            userIdClaim = externalUser.FindFirst(ClaimTypes.NameIdentifier);
        }
        else if (provider == "Microsoft")
        {
            userIdClaim = externalUser.FindFirst(ClaimTypes.NameIdentifier);
        }
        else if (provider == "Google")
        {
            userIdClaim = externalUser.FindFirst(ClaimTypes.NameIdentifier);
        }
        else if (provider == "Facebook")
        {
            userIdClaim = externalUser.FindFirst(ClaimTypes.NameIdentifier);
        }
        else
        {
            userIdClaim = externalUser.FindFirst(ClaimTypes.NameIdentifier);
        }

        var providerUserId = userIdClaim.Value;

        // find external user
        var user = await _userManager.FindByLoginAsync(provider, providerUserId);
        if (user == null)
        {
            // this might be where you might initiate a custom workflow for user registration
            // in this sample we don't show how that would be done, as our sample implementation
            // simply auto-provisions new external user
            user = await AutoProvisionUserAsync(provider, providerUserId, externalUser.Claims);
        }

        // this allows us to collect any additional claims or properties
        // for the specific protocols used and store them in the local auth cookie.
        // this is typically used to store data needed for signout from those protocols.
        var additionalLocalClaims = new List<Claim>();
        var localSignInProps = new AuthenticationProperties();
        CaptureExternalLoginContext(result, additionalLocalClaims, localSignInProps);

        // issue authentication cookie for user
        await _signInManager.SignInWithClaimsAsync(user, localSignInProps, additionalLocalClaims);

        // delete temporary cookie used during external authentication
        await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

        // retrieve return URL
        var returnUrl = result.Properties.Items["returnUrl"] ?? "~/";

        // check if external login is in the context of an OIDC request
        var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
        await _events.RaiseAsync(new UserLoginSuccessEvent(provider, providerUserId, user.Id.ToString(), user.UserName, true, context?.Client.ClientId));

        if (context != null)
        {
            if (context.IsNativeClient())
            {
                // The client is native, so this change in how to
                // return the response is for better UX for the end user.
                return this.LoadingPage(returnUrl);
            }
        }

        return Redirect(returnUrl);
    }

    private async Task<User> AutoProvisionUserAsync(string provider, string providerUserId, IEnumerable<Claim> claims)
    {
        // email
        Claim emailClaim = null;

        if (provider == "AAD")
        {
            emailClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Email) ??
                    claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
        }
        else if (provider == "Microsoft")
        {
            emailClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Email) ??
                     claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
        }
        else if (provider == "Google")
        {
            emailClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Email) ??
                    claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
        }
        else if (provider == "Facebook")
        {
            emailClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Email) ??
                    claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
        }
        else
        {
            emailClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Email) ??
                     claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
        }

        // create a list of claims that we want to transfer into our store
        var filtered = new List<Claim>();

        // user's display name
        var name = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name)?.Value ??
                   claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
        if (name != null)
        {
            filtered.Add(new Claim(JwtClaimTypes.Name, name));
        }
        else
        {
            var first = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value ??
                        claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
            var last = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value ??
                       claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;
            if (first != null && last != null)
            {
                filtered.Add(new Claim(JwtClaimTypes.Name, first + " " + last));
            }
            else if (first != null)
            {
                filtered.Add(new Claim(JwtClaimTypes.Name, first));
            }
            else if (last != null)
            {
                filtered.Add(new Claim(JwtClaimTypes.Name, last));
            }
        }

        var userName = $"{provider}_{providerUserId}";

        var user = await _userManager.FindByNameAsync(userName);

        if (user == null)
        {
            user = new User
            {
                UserName = userName,
                Email = emailClaim?.Value,
            };

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                throw new Exception(createResult.Errors.First().Description);
            }
        }

        if (filtered.Any())
        {
            var addClaimResult = await _userManager.AddClaimsAsync(user, filtered);
            if (!addClaimResult.Succeeded) throw new Exception(addClaimResult.Errors.First().Description);
        }

        var addLoginResult = await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerUserId, provider));
        if (!addLoginResult.Succeeded) throw new Exception(addLoginResult.Errors.First().Description);

        return user;
    }

    // if the external login is OIDC-based, there are certain things we need to preserve to make logout work
    // this will be different for WS-Fed, SAML2p or other protocols
    private void CaptureExternalLoginContext(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
    {
        // capture the idp used to login, so the session knows where the user came from
        localClaims.Add(new Claim(JwtClaimTypes.IdentityProvider, externalResult.Properties.Items["scheme"]));

        // if the external system sent a session id claim, copy it over
        // so we can use it for single sign-out
        var sid = externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
        if (sid != null)
        {
            localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
        }

        // if the external provider issued an id_token, we'll keep it for signout
        var idToken = externalResult.Properties.GetTokenValue("id_token");
        if (idToken != null)
        {
            localSignInProps.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = idToken } });
        }
    }
}