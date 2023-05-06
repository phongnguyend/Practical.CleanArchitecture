using System.Collections.Generic;

namespace ClassifiedAds.WebAPI.Authorization;

public static class AuthorizationPolicyNames
{
    public const string GetAuditLogsPolicy = "GetAuditLogsPolicy";
    public const string GetConfigurationEntriesPolicy = "GetConfigurationEntriesPolicy";
    public const string GetConfigurationEntryPolicy = "GetConfigurationEntryPolicy";
    public const string AddConfigurationEntryPolicy = "AddConfigurationEntryPolicy";
    public const string UpdateConfigurationEntryPolicy = "UpdateConfigurationEntryPolicy";
    public const string DeleteConfigurationEntryPolicy = "DeleteConfigurationEntryPolicy";
    public const string GetFilesPolicy = "GetFilesPolicy";
    public const string UploadFilePolicy = "UploadFilePolicy";
    public const string GetFilePolicy = "GetFilePolicy";
    public const string DownloadFilePolicy = "DownloadFilePolicy";
    public const string UpdateFilePolicy = "UpdateFilePolicy";
    public const string DeleteFilePolicy = "DeleteFilePolicy";
    public const string GetFileAuditLogsPolicy = "GetFileAuditLogsPolicy";
    public const string GetProductsPolicy = "GetProductsPolicy";
    public const string GetProductPolicy = "GetProductPolicy";
    public const string AddProductPolicy = "AddProductPolicy";
    public const string UpdateProductPolicy = "UpdateProductPolicy";
    public const string DeleteProductPolicy = "DeleteProductPolicy";
    public const string GetProductAuditLogsPolicy = "GetProductAuditLogsPolicy";
    public const string GetRolesPolicy = "GetRolesPolicy";
    public const string GetRolePolicy = "GetRolePolicy";
    public const string AddRolePolicy = "AddRolePolicy";
    public const string UpdateRolePolicy = "UpdateRolePolicy";
    public const string DeleteRolePolicy = "DeleteRolePolicy";
    public const string GetUsersPolicy = "GetUsersPolicy";
    public const string GetUserPolicy = "GetUserPolicy";
    public const string AddUserPolicy = "AddUserPolicy";
    public const string UpdateUserPolicy = "UpdateUserPolicy";
    public const string SetPasswordPolicy = "SetPasswordPolicy";
    public const string DeleteUserPolicy = "DeleteUserPolicy";
    public const string SendResetPasswordEmailPolicy = "SendResetPasswordEmailPolicy";
    public const string SendConfirmationEmailAddressEmailPolicy = "SendConfirmationEmailAddressEmailPolicy";

    public static IEnumerable<string> GetPolicyNames()
    {
        yield return GetAuditLogsPolicy;
        yield return GetConfigurationEntriesPolicy;
        yield return GetConfigurationEntryPolicy;
        yield return AddConfigurationEntryPolicy;
        yield return UpdateConfigurationEntryPolicy;
        yield return DeleteConfigurationEntryPolicy;
        yield return GetFilesPolicy;
        yield return UploadFilePolicy;
        yield return GetFilePolicy;
        yield return DownloadFilePolicy;
        yield return UpdateFilePolicy;
        yield return DeleteFilePolicy;
        yield return GetFileAuditLogsPolicy;
        yield return GetProductsPolicy;
        yield return GetProductPolicy;
        yield return AddProductPolicy;
        yield return UpdateProductPolicy;
        yield return DeleteProductPolicy;
        yield return GetProductAuditLogsPolicy;
        yield return GetRolesPolicy;
        yield return GetRolePolicy;
        yield return AddRolePolicy;
        yield return UpdateRolePolicy;
        yield return DeleteRolePolicy;
        yield return GetUsersPolicy;
        yield return GetUserPolicy;
        yield return AddUserPolicy;
        yield return UpdateUserPolicy;
        yield return SetPasswordPolicy;
        yield return DeleteUserPolicy;
        yield return SendResetPasswordEmailPolicy;
        yield return SendConfirmationEmailAddressEmailPolicy;
    }
}
