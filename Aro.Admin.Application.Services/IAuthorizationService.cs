namespace Aro.Admin.Application.Services;

public interface IAuthorizationService
{
    Task<bool> UserHasPermissionAsync(Guid userId, string permissionCode, CancellationToken cancellationToken);
    Task EnsureUserHasPermissionAsync(Guid userId, string permissionCode, CancellationToken cancellationToken);
}

