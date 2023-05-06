using ClassifiedAds.Services.Storage.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Threading.Tasks;

namespace ClassifiedAds.Services.Storage.Authorization;

public class FileEntryAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, FileEntry>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, FileEntry resource)
    {
        // TODO: check CreatedById
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}
