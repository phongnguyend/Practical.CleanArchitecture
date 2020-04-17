using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace ClassifiedAds.WebMVC.Authorization
{
    public class CustomRequirement : IAuthorizationRequirement
    {
    }

    public class CustomRequirementHandler : AuthorizationHandler<CustomRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomRequirement requirement)
        {
            var user = context.User;
            if (user.HasClaim(x => x.Type == "now"))
            {

            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
