using System.Collections.Generic;

namespace ClassifiedAds.Services.Configuration.Authorization;

public static class AuthorizationPolicyNames
{
    public const string GetConfigurationEntriesPolicy = "GetConfigurationEntriesPolicy";
    public const string GetConfigurationEntryPolicy = "GetConfigurationEntryPolicy";
    public const string AddConfigurationEntryPolicy = "AddConfigurationEntryPolicy";
    public const string UpdateConfigurationEntryPolicy = "UpdateConfigurationEntryPolicy";
    public const string DeleteConfigurationEntryPolicy = "DeleteConfigurationEntryPolicy";

    public static IEnumerable<string> GetPolicyNames()
    {
        yield return GetConfigurationEntriesPolicy;
        yield return GetConfigurationEntryPolicy;
        yield return AddConfigurationEntryPolicy;
        yield return UpdateConfigurationEntryPolicy;
        yield return DeleteConfigurationEntryPolicy;
    }
}
