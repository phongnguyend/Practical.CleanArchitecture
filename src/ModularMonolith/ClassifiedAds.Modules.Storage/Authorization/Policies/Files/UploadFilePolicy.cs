using ClassifiedAds.Infrastructure.Web.Authorization.Policies;
using ClassifiedAds.Modules.Storage.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace ClassifiedAds.Modules.Storage.Authorization.Policies.Files
{
    public class UploadFilePolicy : IPolicy
    {
        public void Configure(AuthorizationPolicyBuilder policy)
        {
            policy.RequireAuthenticatedUser();
            policy.AddRequirements(new PermissionRequirement
            {
                Feature = "FilesManagement",
                Action = "Add",
            });
        }
    }
}
