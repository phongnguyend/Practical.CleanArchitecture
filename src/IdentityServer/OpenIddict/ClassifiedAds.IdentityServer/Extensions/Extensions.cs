using OpenIddict.Abstractions;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ClassifiedAds.IdentityServer.Extensions;

public static class Extensions
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

    public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> items)
    {
        var evaluatedItems = new List<T>();
        await foreach (var item in items)
            evaluatedItems.Add(item);
        return evaluatedItems;
    }
}
