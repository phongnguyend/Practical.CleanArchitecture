using ClassifiedAds.Infrastructure.Web.Authorization.Policies;
using ClassifiedAds.Services.AuditLog.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace ClassifiedAds.Services.AuditLog.Authorization.Policies.AuditLogs
{
    public class GetAuditLogsPolicy : IPolicy
    {
        public void Configure(AuthorizationPolicyBuilder policy)
        {
            policy.RequireAuthenticatedUser();
            policy.AddRequirements(new PermissionRequirement
            {
                Feature = "AuditLogsManagement",
                Action = "List",
            });
        }
    }
}
