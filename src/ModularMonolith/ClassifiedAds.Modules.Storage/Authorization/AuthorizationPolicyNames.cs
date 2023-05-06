using System.Collections.Generic;

namespace ClassifiedAds.Modules.Storage.Authorization;

public static class AuthorizationPolicyNames
{
    public const string GetFilesPolicy = "GetFilesPolicy";
    public const string UploadFilePolicy = "UploadFilePolicy";
    public const string GetFilePolicy = "GetFilePolicy";
    public const string DownloadFilePolicy = "DownloadFilePolicy";
    public const string UpdateFilePolicy = "UpdateFilePolicy";
    public const string DeleteFilePolicy = "DeleteFilePolicy";
    public const string GetFileAuditLogsPolicy = "GetFileAuditLogsPolicy";

    public static IEnumerable<string> GetPolicyNames()
    {
        yield return GetFilesPolicy;
        yield return UploadFilePolicy;
        yield return GetFilePolicy;
        yield return DownloadFilePolicy;
        yield return UpdateFilePolicy;
        yield return DeleteFilePolicy;
        yield return GetFileAuditLogsPolicy;
    }
}
