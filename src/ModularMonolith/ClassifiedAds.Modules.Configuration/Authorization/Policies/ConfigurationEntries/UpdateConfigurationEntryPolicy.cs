using ClassifiedAds.Infrastructure.Web.Authorization.Policies;
using ClassifiedAds.Modules.Configuration.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace ClassifiedAds.Modules.Configuration.Authorization.Policies.ConfigurationEntries
{
    public class UpdateConfigurationEntryPolicy : IPolicy
    {
        public void Configure(AuthorizationPolicyBuilder policy)
        {
            policy.RequireAuthenticatedUser();
            policy.AddRequirements(new PermissionRequirement
            {
                Feature = "ConfigurationManagement",
                Action = "Set",
            });
        }
    }
}
