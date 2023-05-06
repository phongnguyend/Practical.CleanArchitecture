using System.Collections.Generic;

namespace ClassifiedAds.Modules.Product.Authorization;

public static class AuthorizationPolicyNames
{
    public const string GetProductsPolicy = "GetProductsPolicy";
    public const string GetProductPolicy = "GetProductPolicy";
    public const string AddProductPolicy = "AddProductPolicy";
    public const string UpdateProductPolicy = "UpdateProductPolicy";
    public const string DeleteProductPolicy = "DeleteProductPolicy";
    public const string GetProductAuditLogsPolicy = "GetProductAuditLogsPolicy";

    public static IEnumerable<string> GetPolicyNames()
    {
        yield return GetProductsPolicy;
        yield return GetProductPolicy;
        yield return AddProductPolicy;
        yield return UpdateProductPolicy;
        yield return DeleteProductPolicy;
        yield return GetProductAuditLogsPolicy;
    }
}
