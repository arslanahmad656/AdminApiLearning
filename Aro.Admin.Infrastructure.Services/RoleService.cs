using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceResponses;
using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Domain.Shared;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Aro.Admin.Infrastructure.Services;

public class RoleService(IRepositoryManager repository, IMapper mapper, IAuthorizationService authorizationService) : IRoleService
{
    private readonly IUserRoleRepository userRoleRepository = repository.UserRoleRepository;
    private readonly IRoleRepository roleRepository = repository.RoleRepository;

    public async Task AssignRolesToUsers(IEnumerable<Guid> userIds, IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default)
    {
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.AssignUserRole], cancellationToken).ConfigureAwait(false);

        var userRolesToAdd = new List<UserRole>();

        foreach (var userId in userIds)
        {
            foreach (var roleId in roleIds)
            {
                userRolesToAdd.Add(new()
                {
                    UserId = userId,
                    RoleId = roleId
                });
            }
        }

        userRolesToAdd.ForEach(async ur => await userRoleRepository.Create(ur, cancellationToken).ConfigureAwait(false));

        await repository.SaveChanges(cancellationToken).ConfigureAwait(false);
    }

    public async Task AssignRolesToUsers(IEnumerable<Guid> userIds, IEnumerable<string> roleNames, CancellationToken cancellationToken = default)
    {
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.AssignUserRole], cancellationToken).ConfigureAwait(false);

        var rolesToAdd = await GetByNames(roleNames, cancellationToken).ConfigureAwait(false);
        var roleIds = rolesToAdd.Select(r => r.Id);

        await AssignRolesToUsers(userIds, roleIds, cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<GetRoleRespose>> GetByNames(IEnumerable<string> names, CancellationToken cancellationToken = default)
    {
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.GetUserRoles], cancellationToken).ConfigureAwait(false);
        var roleEntities = await roleRepository
            .GetByNames(names)
            .ToListAsync(cancellationToken);

        var roles = mapper.Map<List<GetRoleRespose>>(roleEntities);

        return roles;
    }

    public async Task<List<GetRoleRespose>> GetByIds(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.GetUserRoles], cancellationToken).ConfigureAwait(false);
        var roleEntities = await roleRepository
            .GetAll()
            .Where(r => ids.Contains(r.Id))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var roles = mapper.Map<List<GetRoleRespose>>(roleEntities);
        return roles;
    }

    public async Task RevokeRolesFromUsers(IEnumerable<Guid> userIds, IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default)
    {
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.RevokeUserRole], cancellationToken).ConfigureAwait(false);

        var userRoles = await userRoleRepository
            .GetByUserRoles(userIds, roleIds)
            .Where(ur => roleIds.Contains(ur.RoleId))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        userRoleRepository.Remove(userRoles);

        await repository.SaveChanges(cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> UserHasRole(Guid userId, string roleName, CancellationToken cancellationToken = default)
    {
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.TestUserRole], cancellationToken).ConfigureAwait(false);
        var hasRole = await userRoleRepository
            .GetByUserIds([userId])
            .Include(ur => ur.Role)
            .Where(ur => ur.Role.Name == roleName)
            .AnyAsync(cancellationToken)
            .ConfigureAwait(false);

        return hasRole;
    }

    public async Task<bool> UserHasRole(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.TestUserRole], cancellationToken).ConfigureAwait(false);
        var hasRole = await userRoleRepository
            .Exists(userId, roleId, cancellationToken)
            .ConfigureAwait(false);

        return hasRole;
    }

    public async Task<List<GetUserRolesResponse>> GetUserRoles(Guid userId, CancellationToken cancellationToken = default)
    {
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.GetUserRoles], cancellationToken).ConfigureAwait(false);
        var roleEntities = await userRoleRepository
            .GetByUserIds([userId])
            .Include(ur => ur.Role)
            .Select(ur => ur.Role)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var roles = mapper.Map<List<GetUserRolesResponse>>(roleEntities);

        return roles;
    }
}
