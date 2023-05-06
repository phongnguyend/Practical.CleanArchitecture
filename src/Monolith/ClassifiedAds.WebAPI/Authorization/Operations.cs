using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace ClassifiedAds.WebAPI.Authorization;

public static class Operations
{
    public static readonly OperationAuthorizationRequirement Create = new OperationAuthorizationRequirement { Name = nameof(Create) };
    public static readonly OperationAuthorizationRequirement Read = new OperationAuthorizationRequirement { Name = nameof(Read) };
    public static readonly OperationAuthorizationRequirement Update = new OperationAuthorizationRequirement { Name = nameof(Update) };
    public static readonly OperationAuthorizationRequirement Delete = new OperationAuthorizationRequirement { Name = nameof(Delete) };
}
