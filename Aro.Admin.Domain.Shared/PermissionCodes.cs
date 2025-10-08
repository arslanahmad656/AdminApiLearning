namespace Aro.Admin.Domain.Shared;

public class PermissionCodes
{
    #region [Users]
    public readonly string CreateUser = "user.create";
    public readonly string GetUser = "user.get";
    #endregion

    #region [UserRoles]
    public readonly string AssignUserRole = "userrole.assign";
    public readonly string RevokeUserRole = "userrole.revoke";
    public readonly string GetUserRoles = "userrole.get";
    public readonly string TestUserRole = "userrole.test";
    #endregion

    #region [Miscs]
    public readonly string MigrateDabase = "database.migrate";
    public readonly string SeedApplication = "application.seed";
    public readonly string InitializeSystem = "application.initialize";
    #endregion
}
