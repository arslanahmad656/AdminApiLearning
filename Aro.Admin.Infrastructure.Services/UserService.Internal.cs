using Aro.Admin.Application.Services.DTOs.ServiceResponses;
using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Aro.Admin.Infrastructure.Services;

public partial class UserService
{
    public async Task<GetUserResponse> GetUserFromQueryable(IQueryable<User> query, string identifier, bool includeRoles, bool includePasswordHash, CancellationToken cancellationToken = default)
    {
        string[] requiredPermissions = includeRoles ? [permissionCodes.GetUser, permissionCodes.GetUserRoles] : [permissionCodes.GetUser];
        await authorizationService.EnsureCurrentUserPermissions(requiredPermissions, cancellationToken);

        //var query = userRepository.GetByEmail(email);
        if (includeRoles)
        {
            query = query
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role);
        }

        var userEntity = await query.SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false)
            ?? throw new UserNotFoundException(identifier);

        var response = mapper.Map<GetUserResponse>(userEntity);
        if (!includePasswordHash)
        {
            response = response with { PasswordHash = string.Empty };
        }

        return response;
    }
}
