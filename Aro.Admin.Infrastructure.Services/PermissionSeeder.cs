using Aro.Admin.Application.Services.Authorization;
using Aro.Admin.Application.Services.PermissionSeeder;
using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Shared;
using Aro.Common.Application.Repository;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Application.Services.Serializer;
using Aro.Common.Domain.Shared;
using Aro.Common.Domain.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Aro.Admin.Infrastructure.Services;

public class PermissionSeeder
(
    Application.Repository.IRepositoryManager repository,
    IUnitOfWork unitOfWork,
    ISerializer serializer,
    ErrorCodes errorCodes,
    IAuthorizationService authorizationService,
    ILogManager<PermissionSeeder> logger
)
: IPermissionSeeder
{
    public async Task Seed(string jsonFile, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(Seed));

        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.SeedApplication], cancellationToken).ConfigureAwait(false);
        logger.LogDebug("Authorization verified for seeding");

        if (!File.Exists(jsonFile))
        {
            logger.LogError("Seed file not found: {JsonFile}", jsonFile);
            throw new FileNotFoundException(errorCodes.FILE_NOT_FOUND_ERROR, $"Seed file not found: {jsonFile}");
        }
        logger.LogDebug("Seed file exists: {JsonFile}", jsonFile);

        var json = await File.ReadAllTextAsync(jsonFile, cancellationToken);
        logger.LogDebug("Read seed file content, length: {Length}", json.Length);

        var model = serializer.Deserialize<RbacSeedModel>(json)
            ?? throw new AroException(errorCodes.DESERIALIZATION_ERROR, "Failed to deserialize RBAC seed file.");
        logger.LogDebug("Deserialized seed model, permissionCount: {PermissionCount}, roleCount: {RoleCount}", model.Permissions.Count, model.Roles.Count);

        logger.LogDebug("Retrieving existing data from database");
        var existingPermissions = await repository.PermissionRepository.GetAll().ToListAsync(cancellationToken).ConfigureAwait(false);
        var existingRoles = await repository.RoleRepository.GetAll().ToListAsync(cancellationToken).ConfigureAwait(false);
        var existingRolePermissions = await repository.RolePermissionRepository.GetAll().ToListAsync(cancellationToken).ConfigureAwait(false);
        logger.LogDebug("Retrieved existing data, permissions: {PermissionCount}, roles: {RoleCount}, rolePermissions: {RolePermissionCount}", existingPermissions.Count, existingRoles.Count, existingRolePermissions.Count);

        var permissionLookup = existingPermissions.ToDictionary(p => p.Name, p => p);
        var roleLookup = existingRoles.ToDictionary(r => r.Name, r => r);
        var rolePermissionSet = existingRolePermissions
            .Select(rp => (rp.RoleId, rp.PermissionId))
            .ToHashSet();

        logger.LogDebug("Processing {PermissionCount} permissions", model.Permissions.Count);
        foreach (var permModel in model.Permissions)
        {
            if (!permissionLookup.TryGetValue(permModel.Name, out var perm))
            {
                perm = new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = permModel.Name
                };

                await repository.PermissionRepository.Create(perm, cancellationToken);
                permissionLookup[perm.Name] = perm;
                logger.LogDebug("Created new permission: {PermissionName}", perm.Name);
            }
            else
            {
                logger.LogDebug("Permission already exists: {PermissionName}", perm.Name);
            }
        }

        logger.LogDebug("Processing {RoleCount} roles", model.Roles.Count);
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
                logger.LogDebug("Created new role: {RoleName}", role.Name);
            }
            else
            {
                logger.LogDebug("Role already exists: {RoleName}", role.Name);
            }

            logger.LogDebug("Processing {PermissionCount} permissions for role: {RoleName}", roleModel.Permissions.Count, roleModel.Name);
            foreach (var permName in roleModel.Permissions)
            {
                if (!permissionLookup.TryGetValue(permName, out var permission))
                {
                    logger.LogError("Permission not found: {PermissionName} for role: {RoleName}", permName, roleModel.Name);
                    throw new AroException(errorCodes.DATA_SEED_ERROR, $"Permission '{permName}' not found while seeding role '{roleModel.Name}'.");
                }

                if (!rolePermissionSet.Contains((role.Id, permission.Id)))
                {
                    var rp = new RolePermission
                    {
                        RoleId = role.Id,
                        PermissionId = permission.Id
                    };

                    await repository.RolePermissionRepository.Create(rp, cancellationToken);
                    rolePermissionSet.Add((role.Id, permission.Id));
                    logger.LogDebug("Created role permission: {RoleName} -> {PermissionName}", roleModel.Name, permName);
                }
                else
                {
                    logger.LogDebug("Role permission already exists: {RoleName} -> {PermissionName}", roleModel.Name, permName);
                }
            }
        }

        logger.LogDebug("Saving changes to database");
        await unitOfWork.SaveChanges(cancellationToken).ConfigureAwait(false);
        logger.LogInfo("Application seeding completed successfully");

        logger.LogDebug("Completed {MethodName}", nameof(Seed));
    }
}

file class PermissionSeedModel
{
    public string Name { get; set; } = default!;
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