using Microsoft.AspNetCore.Authorization;

namespace ClassifiedAds.Infrastructure.Web.Authorization.Policies;

public interface IPolicy
{
    void Configure(AuthorizationPolicyBuilder policy);
}
