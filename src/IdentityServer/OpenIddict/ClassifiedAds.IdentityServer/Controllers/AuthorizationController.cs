using ClassifiedAds.Domain.Entities;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ClassifiedAds.IdentityServer.Controllers;

public class AuthorizationController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AuthorizationController(
        UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet("~/connect/authorize")]
    [HttpPost("~/connect/authorize")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Authorize()
    {
        var request = HttpContext.GetOpenIddictServerRequest() ??
                      throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        // Retrieve the user principal stored in the authentication cookie.
        var result = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);

        // If the user principal can't be extracted, redirect the user to the login page.
        if (!result.Succeeded)
        {
            return Challenge(
                authenticationSchemes: IdentityConstants.ApplicationScheme,
                properties: new AuthenticationProperties
                {
                    RedirectUri = Request.PathBase + Request.Path + QueryString.Create(
                        Request.HasFormContentType ? Request.Form.ToList() : Request.Query.ToList())
                });
        }

        // Create a new claims principal
        var claims = new List<Claim>
        {
            // 'subject' claim which is required
            new Claim(OpenIddictConstants.Claims.Subject, result.Principal.GetClaim(ClaimTypes.NameIdentifier)),
            new Claim("some claim", "some value").SetDestinations(OpenIddictConstants.Destinations.AccessToken),
            new Claim(OpenIddictConstants.Claims.Email, "some@email").SetDestinations(OpenIddictConstants.Destinations.IdentityToken)
        };

        var claimsIdentity = new ClaimsIdentity(claims, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        // Set requested scopes (this is not done automatically)
        claimsPrincipal.SetScopes(request.GetScopes());

        // Signing in with the OpenIddict authentiction scheme trigger OpenIddict to issue a code (which can be exchanged for an access token)
        return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    [HttpPost("~/connect/token")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest() ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        ClaimsPrincipal claimsPrincipal;

        if (request.IsClientCredentialsGrantType())
        {
            // Note: the client credentials are automatically validated by OpenIddict:
            // if client_id or client_secret are invalid, this action won't be invoked.

            var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            // Subject (sub) is a required field, we use the client id as the subject identifier here.
            identity.AddClaim(OpenIddictConstants.Claims.Subject, request.ClientId ?? throw new InvalidOperationException());

            // Add some claim, don't forget to add destination otherwise it won't be added to the access token.
            identity.AddClaim("some-claim", "some-value", OpenIddictConstants.Destinations.AccessToken);

            claimsPrincipal = new ClaimsPrincipal(identity);

            claimsPrincipal.SetScopes(request.GetScopes());
        }
        else if (request.IsAuthorizationCodeGrantType())
        {
            // Retrieve the claims principal stored in the authorization code
            claimsPrincipal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;
        }
        else if (request.IsRefreshTokenGrantType())
        {
            // Retrieve the claims principal stored in the refresh token.
            claimsPrincipal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;
        }
        else if (request.IsPasswordGrantType())
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                var properties = new AuthenticationProperties(new Dictionary<string, string>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The username/password couple is invalid."
                });

                return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            // Validate the username/password parameters and ensure the account is not locked out.
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                var properties = new AuthenticationProperties(new Dictionary<string, string>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The username/password couple is invalid."
                });

                return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            // Create the claims-based identity that will be used by OpenIddict to generate tokens.
            var identity = new ClaimsIdentity(
                authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                nameType: OpenIddictConstants.Claims.Name,
                roleType: OpenIddictConstants.Claims.Role);

            // Add the claims that will be persisted in the tokens.
            identity.SetClaim(OpenIddictConstants.Claims.Subject, await _userManager.GetUserIdAsync(user))
                    .SetClaim(OpenIddictConstants.Claims.Email, await _userManager.GetEmailAsync(user))
                    .SetClaim(OpenIddictConstants.Claims.Name, await _userManager.GetUserNameAsync(user));

            // Set the list of scopes granted to the client application.
            identity.SetScopes(new[]
            {
                OpenIddictConstants.Scopes.OpenId,
                OpenIddictConstants.Scopes.Email,
                OpenIddictConstants.Scopes.Profile,
                OpenIddictConstants.Scopes.Roles
            }.Intersect(request.GetScopes()));

            identity.SetDestinations(GetDestinations);

            return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        else
        {
            throw new InvalidOperationException("The specified grant type is not supported.");
        }

        // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
        return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    [HttpGet("~/connect/logout")]
    public IActionResult Logout() => View();

    [ActionName(nameof(Logout))]
    [HttpPost("~/connect/logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogoutPost()
    {
        // Ask ASP.NET Core Identity to delete the local and external cookies created
        // when the user agent is redirected from the external identity provider
        // after a successful authentication flow (e.g Google or Facebook).
        await _signInManager.SignOutAsync();

        // Returning a SignOutResult will ask OpenIddict to redirect the user agent
        // to the post_logout_redirect_uri specified by the client application or to
        // the RedirectUri specified in the authentication properties if none was set.
        return SignOut(
            authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            properties: new AuthenticationProperties
            {
                RedirectUri = "/"
            });
    }

    private static IEnumerable<string> GetDestinations(Claim claim)
    {
        // Note: by default, claims are NOT automatically included in the access and identity tokens.
        // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
        // whether they should be included in access tokens, in identity tokens or in both.

        switch (claim.Type)
        {
            case OpenIddictConstants.Claims.Name:
                yield return OpenIddictConstants.Destinations.AccessToken;

                if (claim.Subject.HasScope(OpenIddictConstants.Scopes.Profile))
                {
                    yield return OpenIddictConstants.Destinations.IdentityToken;
                }

                yield break;

            case OpenIddictConstants.Claims.Email:
                yield return OpenIddictConstants.Destinations.AccessToken;

                if (claim.Subject.HasScope(OpenIddictConstants.Scopes.Email))
                {
                    yield return OpenIddictConstants.Destinations.IdentityToken;
                }

                yield break;

            case OpenIddictConstants.Claims.Role:
                yield return OpenIddictConstants.Destinations.AccessToken;

                if (claim.Subject.HasScope(OpenIddictConstants.Scopes.Roles))
                {
                    yield return OpenIddictConstants.Destinations.IdentityToken;
                }

                yield break;

            // Never include the security stamp in the access and identity tokens, as it's a secret value.
            case "AspNet.Identity.SecurityStamp": yield break;

            default:
                yield return OpenIddictConstants.Destinations.AccessToken;
                yield break;
        }
    }
}
