using System.Collections.Generic;

namespace ClassifiedAds.Services.AuditLog.Authorization;

public static class AuthorizationPolicyNames
{
    public const string GetAuditLogsPolicy = "GetAuditLogsPolicy";

    public static IEnumerable<string> GetPolicyNames()
    {
        yield return GetAuditLogsPolicy;
    }
}
