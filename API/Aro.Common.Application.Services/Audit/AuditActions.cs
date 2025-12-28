namespace Aro.Common.Application.Services.Audit;

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
    public readonly string AccessTokenRefreshed = "ACCESS_TOKEN_REFRESHED";

    #region [Entity Creation]

    public readonly string UserCreated = "USER_CREATED";
    public readonly string GroupCreated = "GROUP_CREATED";
    public readonly string PropertyCreated = "PROPERTY_CREATED";
    public readonly string RoomCreated = "ROOM_CREATED";
    public readonly string AmenityCreated = "AMENITY_CREATED";
    public readonly string PolicyCreated = "POLICY_CREATED";
    public readonly string FileCreated = "FILE_CREATED";
    public readonly string CountriesCreated = "COUNTRIES_CREATED";

    #endregion

    #region [Entity Patches]

    public readonly string GroupPatched = "GROUP_PATCHED";
    public readonly string RoomPatched = "ROOM_PATCHED";
    public readonly string AmenityPatched = "AMENITY_PATCHED";
    public readonly string PolicyPatched = "POLICY_PATCHED";

    #endregion

    #region [Entity Status]

    public readonly string RoomActivated = "ROOM_ACTIVATED";
    public readonly string RoomDeactivated = "ROOM_DEACTIVATED";

    #endregion

    #region [Entity Deleted]

    public readonly string GroupDeleted = "GROUP_DELETED";
    public readonly string RoomDeleted = "ROOM_DELETED";
    public readonly string AmenityDeleted = "AMENITY_DELETED";
    public readonly string PolicyDeleted = "POLICY_DELETED";

    #endregion
}
