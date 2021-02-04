using ClassifiedAds.Infrastructure.Web.Authorization.Policies;
using ClassifiedAds.WebAPI.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace ClassifiedAds.WebAPI.Authorization.Policies.Files
{
    public class DownloadFilePolicy : IPolicy
    {
        public void Configure(AuthorizationPolicyBuilder policy)
        {
            policy.RequireAuthenticatedUser();
            policy.AddRequirements(new PermissionRequirement
            {
                Feature = "FilesManagement",
                Action = "Download",
            });
        }
    }
}
