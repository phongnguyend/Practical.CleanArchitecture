using System.Collections.Generic;

namespace ClassifiedAds.Modules.Identity.Authorization;

public static class AuthorizationPolicyNames
{
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
