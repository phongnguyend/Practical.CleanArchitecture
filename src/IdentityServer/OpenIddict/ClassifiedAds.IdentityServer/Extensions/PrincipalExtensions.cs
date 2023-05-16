using OpenIddict.Abstractions;
using System.Security.Claims;

namespace ClassifiedAds.IdentityServer.Extensions;

public static class PrincipalExtensions
{
    public static string GetDisplayName(this ClaimsPrincipal principal)
    {
        var name = principal.Identity.Name;
        if (!string.IsNullOrEmpty(name))
        {
            return name;
        }

        var sub = principal.FindFirst(OpenIddictConstants.Claims.Subject);
        if (sub != null)
        {
            return sub.Value;
        }

        return string.Empty;
    }
}
