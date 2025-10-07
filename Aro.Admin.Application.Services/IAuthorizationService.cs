namespace Aro.Admin.Application.Services;

public interface IAuthorizationService
{
    Task<bool> UserHasPermission(Guid userId, string permissionCode, CancellationToken cancellationToken);
    
    Task EnsureUserHasPermission(Guid userId, string permissionCode, CancellationToken cancellationToken);

    Task EnsureCurrentUserPermissions(IEnumerable<string> permissions, CancellationToken cancellationToken = default);
}

