namespace Aro.Admin.Domain.Shared;

public class PermissionCodes
{
    #region [Users]
    public const string CreateUser = "user.create";
    public const string GetUser = "user.get";
    #endregion

    #region [UserRoles]
    public const string AssignUserRole = "userrole.assign";
    public const string RevokeUserRole = "userrole.revoke";
    public const string GetUserRoles = "userrole.get";
    public const string TestUserRole = "userrole.test";
    #endregion

    #region [Miscs]
    public const string MigrateDabase = "database.migrate";
    public const string SeedApplication = "application.seed";
    public const string InitializeSystem = "application.initialize";
    #endregion
}


/// REMARKS
/// Normally, I am against using the static fields (consts are also static) because those are GC roots and hence are never collected.
/// However, I am making an exception here since I have to use these fields as the arguments to the PermissionAttribute and attributes expect only consts or literals.