namespace Aro.Common.Domain.Shared;

public class PermissionCodes
{
    #region [Users]
    public const string CreateUser = "user.create";
    public const string GetUser = "user.read";
    public const string ResetPassword = "user.resetpassword";
    public const string ChangePassword = "user.changepassword";
    #endregion

    #region [UserRoles]
    public const string AssignUserRole = "userrole.assign";
    public const string RevokeUserRole = "userrole.revoke";
    public const string GetUserRoles = "userrole.read";
    public const string TestUserRole = "userrole.test";
    #endregion

    #region [Groups]
    public const string CreateGroup = "group.create";
    public const string GetGroups = "group.read";
    public const string GetGroup = "group.read";
    public const string PatchGroup = "group.patch";
    public const string DeleteGroup = "group.delete";
    #endregion

    #region [Properties]
    public const string CreateProperty = "property.create";
    public const string GetProperties = "property.read";
    public const string GetProperty = "property.read";
    public const string PatchProperty = "property.patch";
    public const string DeleteProperty = "property.delete";
    #endregion

    #region [Rooms]
    public const string CreateRoom = "room.create";
    public const string GetRooms = "room.read";
    public const string GetRoom = "room.read";
    public const string EditRoom = "room.edit";
    public const string DeleteRoom = "room.delete";
    #endregion

    #region [Amenities]
    public const string CreateAmenity = "amenity.create";
    public const string GetAmenities = "amenity.read";
    public const string GetAmenity = "amenity.read";
    public const string PatchAmenity = "amenity.patch";
    public const string DeleteAmenity = "amenity.delete";
    #endregion

    #region [Policies]
    public const string CreatePolicy = "policy.create";
    public const string GetPolicies = "policy.read";
    public const string GetPolicy = "policy.read";
    public const string PatchPolicy = "policy.patch";
    public const string DeletePolicy = "policy.delete";
    #endregion

    #region [Miscs]
    public const string MigrateDabase = "database.migrate";
    public const string SeedApplication = "application.seed";
    public const string InitializeSystem = "application.initialize";
    public const string GetSystemSettings = "applicationsettings.read";
    #endregion
}


/// REMARKS
/// Normally, I am against using the static fields (consts are also static) because those are GC roots and hence are never collected.
/// However, I am making an exception here since I have to use these fields as the arguments to the PermissionAttribute and attributes expect only consts or literals.