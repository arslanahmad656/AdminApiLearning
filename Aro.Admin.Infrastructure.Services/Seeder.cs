using Aro.Admin.Application.Services;
using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Domain.Shared;
using Aro.Admin.Domain.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Aro.Admin.Infrastructure.Services;

public class Seeder
(
    IRepositoryManager repository,
    ISerializer serializer,
    ErrorCodes errorCodes,
    IAuthorizationService authorizationService,
    PermissionCodes permissionCodes
)
: ISeeder
{
    public async Task Seed(string jsonFile, CancellationToken cancellationToken = default)
    {
        await authorizationService.EnsureCurrentUserPermissions([permissionCodes.SeedApplication], cancellationToken).ConfigureAwait(false);

        if (!File.Exists(jsonFile))
            throw new FileNotFoundException(errorCodes.FILE_NOT_FOUND_ERROR, $"Seed file not found: {jsonFile}");

        var json = await File.ReadAllTextAsync(jsonFile, cancellationToken);
        var model = serializer.Deserialize<RbacSeedModel>(json)
            ?? throw new AroException(errorCodes.DESERIALIZATION_ERROR, "Failed to deserialize RBAC seed file.");


        var existingPermissions = await repository.PermissionRepository.GetAll().ToListAsync(cancellationToken).ConfigureAwait(false);
        var existingRoles = await repository.RoleRepository.GetAll().ToListAsync(cancellationToken).ConfigureAwait(false);
        var existingRolePermissions = await repository.RolePermissionRepository.GetAll().ToListAsync(cancellationToken).ConfigureAwait(false);

        var permissionLookup = existingPermissions.ToDictionary(p => p.Name, p => p);
        var roleLookup = existingRoles.ToDictionary(r => r.Name, r => r);
        var rolePermissionSet = existingRolePermissions
            .Select(rp => (rp.RoleId, rp.PermissionId))
            .ToHashSet();

        foreach (var permModel in model.Permissions)
        {
            if (!permissionLookup.TryGetValue(permModel.Name, out var perm))
            {
                perm = new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = permModel.Name,
                    Resource = permModel.Resource,
                    Action = permModel.Action
                };

                await repository.PermissionRepository.Create(perm, cancellationToken);
                permissionLookup[perm.Name] = perm;
            }
        }

        foreach (var roleModel in model.Roles)
        {
            if (!roleLookup.TryGetValue(roleModel.Name, out var role))
            {
                role = new Role
                {
                    Id = Guid.NewGuid(),
                    Name = roleModel.Name,
                    Description = roleModel.Description,
                    IsBuiltin = true
                };

                await repository.RoleRepository.Create(role, cancellationToken);
                roleLookup[role.Name] = role;
            }

            foreach (var permName in roleModel.Permissions)
            {
                if (!permissionLookup.TryGetValue(permName, out var permission))
                    throw new AroException(errorCodes.DATA_SEED_ERROR, $"Permission '{permName}' not found while seeding role '{roleModel.Name}'.");

                if (!rolePermissionSet.Contains((role.Id, permission.Id)))
                {
                    var rp = new RolePermission
                    {
                        RoleId = role.Id,
                        PermissionId = permission.Id
                    };

                    await repository.RolePermissionRepository.Create(rp, cancellationToken);
                    rolePermissionSet.Add((role.Id, permission.Id));
                }
            }
        }

        await repository.SaveChanges(cancellationToken).ConfigureAwait(false);
    }
}

file class PermissionSeedModel
{
    public string Name { get; set; } = default!;
    public string Resource { get; set; } = default!;
    public string Action { get; set; } = default!;
    public string Description { get; set; } = default!;
}

file class RoleSeedModel
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public List<string> Permissions { get; set; } = [];
}

file class RbacSeedModel
{
    public List<PermissionSeedModel> Permissions { get; set; } = [];
    public List<RoleSeedModel> Roles { get; set; } = [];
}