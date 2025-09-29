using Aro.Admin.Application.Services;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Aro.Admin.Infrastructure.Services;

public class AuthorizationService(IRepositoryManager repository) : IAuthorizationService
{
    public async Task EnsureUserHasPermissionAsync(Guid userId, string permissionCode, CancellationToken cancellationToken)
    {
        var hasPermission = await UserHasPermissionAsync(userId, permissionCode, cancellationToken).ConfigureAwait(false);
        if (!hasPermission)
        {
            throw new UserPermissionException(userId, permissionCode);
        }
    }

    public async Task<bool> UserHasPermissionAsync(Guid userId, string permissionCode, CancellationToken cancellationToken)
    {
        var exists = await repository.UserRepository
            .GetById(userId)
            .SelectMany(u => u.UserRoles)
            .SelectMany(ur => ur.Role.RolePermissions)
            .AnyAsync(rp => rp.Permission.Name == permissionCode, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return exists;
    }
}
