using Aro.Common.Application.Shared;

namespace Aro.Common.Application.Services.Authorization;

public interface IAuthorizationService : IService
{
    Task<bool> UserHasPermission(Guid userId, string permissionCode, CancellationToken cancellationToken);

    Task<bool> CurrentUserHasPermissions(IEnumerable<string> permissions, CancellationToken cancellationToken = default);

    Task EnsureUserHasPermission(Guid userId, string permissionCode, CancellationToken cancellationToken);

    Task EnsureCurrentUserPermissions(IEnumerable<string> permissions, CancellationToken cancellationToken = default);
}

