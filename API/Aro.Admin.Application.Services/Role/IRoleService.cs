using Aro.Common.Application.Shared;

namespace Aro.Admin.Application.Services.Role;

public interface IRoleService : IService
{
    /// <summary>
    /// Assigns roles to the users.
    /// </summary>
    /// <param name="userIds">Users to which the roles will be assigned.</param>
    /// <param name="roleIds">Roles which will be assigned.</param>
    /// <returns></returns>
    Task AssignRolesToUsers(IEnumerable<Guid> userIds, IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns roles to the users.
    /// </summary>
    /// <param name="userIds">Users to which the roles will be assigned.</param>
    /// <param name="roleNames">Among this list, those roles will be assigned which are available in the database.</param>
    /// <returns></returns>
    Task AssignRolesToUsers(IEnumerable<Guid> userIds, IEnumerable<string> roleNames, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resolves the roles by names.
    /// </summary>
    /// <param name="names"></param>
    /// <returns></returns>
    Task<List<GetRoleRespose>> GetByNames(IEnumerable<string> names, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get roles by ids.
    /// </summary>
    /// <param name="ids"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<GetRoleRespose>> GetByIds(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove the roles from the users. Roles are determined by their ids.
    /// </summary>
    /// <param name="userIds"></param>
    /// <param name="roleIds"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RevokeRolesFromUsers(IEnumerable<Guid> userIds, IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether the user has the specified role. Role is determined by its name.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="roleName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> UserHasRole(Guid userId, string roleName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether the user has the specified role. Role is determined by its id.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="roleId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> UserHasRole(Guid userId, Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all of the roles assigned to the user.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<GetUserRolesResponse>> GetUserRoles(Guid userId, CancellationToken cancellationToken = default);
}
