using Aro.Admin.Application.Services.Authorization;
using Aro.Admin.Application.Services.SystemContext;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Domain.Shared.Exceptions;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Aro.Admin.Infrastructure.Services;

public partial class AuthorizationService(IRepositoryManager repository, ICurrentUserService currentUserService, ISystemContext systemContext, ErrorCodes errorCodes, ILogManager<AuthorizationService> logger) : IAuthorizationService
{
    public async Task EnsureUserHasPermission(Guid userId, string permissionCode, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting {MethodName}", nameof(EnsureUserHasPermission));

        if (IsSystemContext())
        {
            return;
        }

        var hasPermission = await UserHasPermission(userId, permissionCode, cancellationToken).ConfigureAwait(false);
        logger.LogDebug("Permission check result for user: {UserId}, permission: {PermissionCode}, hasPermission: {HasPermission}", userId, permissionCode, hasPermission);
        
        if (!hasPermission)
        {
            logger.LogWarn("User does not have required permission, userId: {UserId}, permissionCode: {PermissionCode}", userId, permissionCode);
            throw new AroUserPermissionException(userId, permissionCode);
        }
        
        logger.LogInfo("User has required permission, userId: {UserId}, permissionCode: {PermissionCode}", userId, permissionCode);
        
        logger.LogDebug("Completed {MethodName}", nameof(EnsureUserHasPermission));
    }

    public async Task<bool> UserHasPermission(Guid userId, string permissionCode, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting {MethodName}", nameof(UserHasPermission));

        if (IsSystemContext())
        {
            return true;
        }

        var exists = await repository.UserRepository
            .GetById(userId)
            .SelectMany(u => u.UserRoles)
            .SelectMany(ur => ur.Role.RolePermissions)
            .AnyAsync(rp => rp.Permission.Name == permissionCode, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        logger.LogDebug("Permission check completed, userId: {UserId}, permissionCode: {PermissionCode}, hasPermission: {HasPermission}", userId, permissionCode, exists);
        
        logger.LogDebug("Completed {MethodName}", nameof(UserHasPermission));
        return exists;
    }

    // TODO: This method suffers from N + 1 query problem. Fix this.
    public async Task<bool> CurrentUserHasPermissions(IEnumerable<string> permissions, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(CurrentUserHasPermissions));

        if (IsSystemContext())
        {
            return true;
        }

        if (!currentUserService.IsAuthenticated())
        {
            logger.LogDebug("Current user is not authenticated");
            return false;
        }

        var currentUserId = currentUserService.GetCurrentUserId();
        if (!currentUserId.HasValue)
        {
            logger.LogDebug("Current user ID is not available");
            return false;
        }

        logger.LogDebug("Checking permissions for current user: {UserId}", currentUserId.Value);
        foreach (var permission in permissions)
        {
            var hasPermission = await UserHasPermission(currentUserId.Value, permission, cancellationToken).ConfigureAwait(false);
            logger.LogDebug("Permission check for current user: {UserId}, permission: {Permission}, hasPermission: {HasPermission}", currentUserId.Value, permission, hasPermission);
            
            if (!hasPermission)
            {
                logger.LogDebug("Current user missing required permission: {Permission}", permission);
                return false;
            }
        }

        logger.LogDebug("Current user has all required permissions: {UserId}", currentUserId.Value);
        
        logger.LogDebug("Completed {MethodName}", nameof(CurrentUserHasPermissions));
        return true;
    }

    // TODO: This method suffers from N + 1 query problem. Fix this.
    public async Task EnsureCurrentUserPermissions(IEnumerable<string> permissions, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(EnsureCurrentUserPermissions));

        if (IsSystemContext())
        {
            return;
        }

        if (!currentUserService.IsAuthenticated())
        {
            logger.LogWarn("Current user is not authenticated");
            throw new AroException(errorCodes.USER_NOT_AUTHENTICATED, $"User is not authenticated.");
        }

        var currentUserId = currentUserService.GetCurrentUserId();
        if (!currentUserId.HasValue)
        {
            logger.LogWarn("Current user ID is not available");
            throw new AroException(errorCodes.USER_NOT_AUTHENTICATED, $"User is not authenticated.");
        }

        logger.LogDebug("Ensuring permissions for current user: {UserId}", currentUserId.Value);
        foreach (var permission in permissions)
        {
            logger.LogDebug("Ensuring permission for current user: {UserId}, permission: {Permission}", currentUserId.Value, permission);
            await EnsureUserHasPermission(currentUserId.Value, permission, cancellationToken).ConfigureAwait(false);
        }
        
        logger.LogInfo("Current user has all required permissions: {UserId}", currentUserId.Value);
        
        logger.LogDebug("Completed {MethodName}", nameof(EnsureCurrentUserPermissions));
    }
}
