using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.Web.Authorization.Requirements;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string PermissionName { get; set; }
}

public class PermissionRequirementHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var user = context.User;

        if (!user.Identity.IsAuthenticated)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        // TODO: check claims
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}
