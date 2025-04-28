using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Hybrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Web.ClaimsTransformations;

public class CustomClaimsTransformation : IClaimsTransformation
{
    private readonly HybridCache _cache;

    public CustomClaimsTransformation(HybridCache cache)
    {
        _cache = cache;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identity = principal.Identities.FirstOrDefault(x => x.IsAuthenticated);
        if (identity == null)
        {
            return principal;
        }

        var userClaim = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

        if (Guid.TryParse(userClaim?.Value, out var userId))
        {
            var issuedAt = principal.Claims.FirstOrDefault(x => x.Type == "iat").Value;

            var cacheKey = $"permissions/{userId}/{issuedAt}";

            var permissions = await _cache.GetOrCreateAsync(cacheKey,
                async (cancellationToken) => await GetPermissionsAsync(userId, cancellationToken),
                tags: ["permissions", $"permissions/{userId}"]);

            var claims = new List<Claim>();
            claims.AddRange(permissions.Select(p => new Claim("Permission", p)));
            claims.AddRange(principal.Claims);

            var newIdentity = new ClaimsIdentity(claims, identity.AuthenticationType);
            return new ClaimsPrincipal(newIdentity);
        }

        return principal;
    }

    private Task<List<string>> GetPermissionsAsync(Guid userId, CancellationToken cancellationToken)
    {
        // TODO: Get from Db
        var claims = new List<string>();
        return Task.FromResult(claims);
    }
}
