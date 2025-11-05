namespace Aro.Admin.Domain.Shared.Audit;

public class AuditActions
{
    public readonly string ApplicationSeeded = "APPLICATION_SEEDED";
    public readonly string MigrationsApplied = "MIGRATIONS_APPLIED";
    public readonly string SystemInitialized = "SYSTEM_INITIALIZED";
    public readonly string RolesAssignedToUsers = "ROLES_ASSIGNED_TO_USERS";
    public readonly string RolesRevokedFromUsers = "ROLES_REVOKED_FROM_USERS";
    public readonly string AuthenticationSuccessful = "AUTHENTICAITON_SUCCESS";
    public readonly string AuthenticationFailed = "AUTHENTICAITON_FAIL";
    public readonly string UserSessionLoggedOut = "SESSION_LOGOUT";
    public readonly string PasswordResetTokenGenerated = "PASSWORD_RESET_TOKEN_GENERATED";
    public readonly string PasswordResetCompleted = "PASSWORD_RESET_COMPLETED";
    public readonly string PasswordResetFailed = "PASSWORD_RESET_FAILED";
    public readonly string PasswordResetLinkGenerated = "PASSWORD_RESET_LINK_GENERATED";
    public readonly string PasswordResetLinkGenerationFailed = "PASSWORD_RESET_LINK_GENERATION_FAILED";
    public readonly string PasswordChangeSuccess = "PASSWORD_CHANGE_SUCCESSFUL";
    public readonly string PasswordChangeFailed = "PASSWORD_CHANGE_FAILED";

    #region [Entity Creation]

    public readonly string UserCreated = "USER_CREATED";
    public readonly string GroupCreated = "GROUP_CREATED";

    #endregion

    #region [Entity Patches]

    public readonly string GroupPatched = "GROUP_PATCHED";

    #endregion

    #region [Entity Deleted]

    public readonly string GroupDeleted = "GROUP_DELETED";

    #endregion
}
