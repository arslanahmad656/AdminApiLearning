using Aro.Admin.Application.Services;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Domain.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Aro.Admin.Infrastructure.Services;

public class AuthorizationService(IRepositoryManager repository, ICurrentUserService currentUserService, ErrorCodes errorCodes) : IAuthorizationService
{
    public async Task EnsureUserHasPermission(Guid userId, string permissionCode, CancellationToken cancellationToken)
    {
        var hasPermission = await UserHasPermission(userId, permissionCode, cancellationToken).ConfigureAwait(false);
        if (!hasPermission)
        {
            throw new UserPermissionException(userId, permissionCode);
        }
    }

    public async Task<bool> UserHasPermission(Guid userId, string permissionCode, CancellationToken cancellationToken)
    {
        var exists = await repository.UserRepository
            .GetById(userId)
            .SelectMany(u => u.UserRoles)
            .SelectMany(ur => ur.Role.RolePermissions)
            .AnyAsync(rp => rp.Permission.Name == permissionCode, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return exists;
    }

    // TODO: This method suffers from N + 1 query problem. Fix this.
    public async Task<bool> CurrentUserHasPermissions(IEnumerable<string> permissions, CancellationToken cancellationToken = default)
    {
        if (!currentUserService.IsAuthenticated())
        {
            return false;
        }

        var currentUserId = currentUserService.GetCurrentUserId();
        if (!currentUserId.HasValue)
        {
            return false;
        }

        foreach (var permission in permissions)
        {
            var hasPermission = await UserHasPermission(currentUserId.Value, permission, cancellationToken).ConfigureAwait(false);
            if (!hasPermission)
            {
                return false;
            }
        }

        return true;
    }

    // TODO: This method suffers from N + 1 query problem. Fix this.
    public async Task EnsureCurrentUserPermissions(IEnumerable<string> permissions, CancellationToken cancellationToken = default)
    {
        if (!currentUserService.IsAuthenticated())
        {
            throw new AroException(errorCodes.USER_NOT_AUTHENTICATED, $"User is not authenticated.");
        }

        var currentUserId = currentUserService.GetCurrentUserId();
        if (!currentUserId.HasValue)
        {
            throw new AroException(errorCodes.USER_NOT_AUTHENTICATED, $"User is not authenticated.");
        }

        foreach (var permission in permissions)
        {
            await EnsureUserHasPermission(currentUserId.Value, permission, cancellationToken).ConfigureAwait(false);
        }
    }
}
