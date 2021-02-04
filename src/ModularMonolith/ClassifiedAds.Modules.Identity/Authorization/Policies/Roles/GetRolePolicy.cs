using ClassifiedAds.Infrastructure.Web.Authorization.Policies;
using ClassifiedAds.Modules.Identity.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace ClassifiedAds.Modules.Identity.Authorization.Policies.Roles
{
    public class GetRolePolicy : IPolicy
    {
        public void Configure(AuthorizationPolicyBuilder policy)
        {
            policy.RequireAuthenticatedUser();
            policy.AddRequirements(new PermissionRequirement
            {
                Feature = "RolesManagement",
                Action = "Get",
            });
        }
    }
}
