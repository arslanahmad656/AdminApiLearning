using Aro.Admin.Application.Services.DTOs.ServiceResponses;

namespace Aro.Admin.Application.Services.DataServices;

public interface IRoleService
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
}
