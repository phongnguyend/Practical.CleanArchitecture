using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ClassifiedAds.WebMVC.ClaimsTransformations
{
    public class CustomClaimsTransformation : IClaimsTransformation
    {
        public CustomClaimsTransformation()
        {
        }

        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var identity = principal.Identities.FirstOrDefault(x => x.IsAuthenticated);
            if (identity == null)
            {
                return Task.FromResult(principal);
            }

            var claims = new List<Claim>
            {
                new Claim("now", DateTime.Now.ToString(), ClaimValueTypes.String, "CustomClaimsTransformation"),
            };

            if (identity.HasClaim(c => c.Type == "now"))
            {
                identity.RemoveClaim(identity.FindFirst(c => c.Type == "now"));
            }

            claims.AddRange(identity.Claims);
            var newIdentity = new ClaimsIdentity(claims, identity.AuthenticationType);
            return Task.FromResult(new ClaimsPrincipal(newIdentity));
        }
    }
}
