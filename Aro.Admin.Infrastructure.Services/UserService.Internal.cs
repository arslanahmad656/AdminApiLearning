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
}
