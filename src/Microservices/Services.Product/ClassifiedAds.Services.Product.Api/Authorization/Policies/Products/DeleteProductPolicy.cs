using ClassifiedAds.Infrastructure.Web.Authorization.Policies;
using ClassifiedAds.Services.Product.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace ClassifiedAds.Services.Product.Authorization.Policies.Products
{
    public class DeleteProductPolicy : IPolicy
    {
        public void Configure(AuthorizationPolicyBuilder policy)
        {
            policy.RequireAuthenticatedUser();
            policy.AddRequirements(new PermissionRequirement
            {
                Feature = "ProductsManagement",
                Action = "Delete",
            });
        }
    }
}
