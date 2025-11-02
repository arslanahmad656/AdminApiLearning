using Aro.Admin.Application.Services.DTOs.ServiceResponses;
using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Shared;
using Aro.Admin.Domain.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Aro.Admin.Infrastructure.Services;

public partial class UserService
{
    public async Task<GetUserResponse> GetUserFromQueryable(IQueryable<User> query, string identifier, bool includeRoles, bool includePasswordHash, CancellationToken cancellationToken = default)
    {
        string[] requiredPermissions = includeRoles ? [PermissionCodes.GetUser, PermissionCodes.GetUserRoles] : [PermissionCodes.GetUser];
        await authorizationService.EnsureCurrentUserPermissions(requiredPermissions, cancellationToken);

        //var query = userRepository.GetByEmail(email);
        if (includeRoles)
        {
            query = query
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role);
        }

        // TODO: fix later:
        query = query
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role);

        var userEntity = await query.SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false)
            ?? throw new AroUserNotFoundException(identifier);

        var response = new GetUserResponse(userEntity.Id, userEntity.Email, userEntity.IsActive, userEntity.DisplayName, userEntity.PasswordHash, userEntity.UserRoles.Select(ur => new GetRoleRespose(ur.Role.Id, ur.Role.Name, ur.Role.Description, ur.Role.IsBuiltin)).ToList());

        if (!includePasswordHash)
        {
            response = response with { PasswordHash = string.Empty };
        }

        return response;
    }

    private async Task ValidatePasswordComplexity(string password, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Validating password complexity for new user");
        var passwordValidationResult = await passwordComplexityService.Validate(password).ConfigureAwait(false);
        if (!passwordValidationResult.Success)
        {
            logger.LogWarn("Password complexity validation failed: {Errors}", string.Join(", ", passwordValidationResult.Errors!));
            throw new AroInvalidOperationException(errorCodes.PASSWORD_COMPLEXITY_ERROR, string.Join(", ", passwordValidationResult.Errors!));
        }

        logger.LogDebug("Password complexity validation passed");
    }

    private async Task RecordPassword(Guid userId, string passwordHash, bool trimHistory = false)
    {
        logger.LogDebug("Recording password in history for user: {UserId}", userId);
        await passwordHistoryEnforcer.RecordPassword(userId, passwordHash).ConfigureAwait(false);
        logger.LogDebug("Password recorded in history for user: {UserId}", userId);

        if (trimHistory)
        {
            await passwordHistoryEnforcer.TrimHistory(userId).ConfigureAwait(false);
        }
    }

    private async Task ValidatePasswordHistory(Guid userId, string newPassword)
    {
        logger.LogDebug("Validating password history for user: {UserId}", userId);
        await passwordHistoryEnforcer.EnsureCanUsePassword(userId, newPassword).ConfigureAwait(false);
        logger.LogDebug("Password history validation passed for user: {UserId}", userId);
    }
}
