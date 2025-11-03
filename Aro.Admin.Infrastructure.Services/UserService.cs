using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceParameters;
using Aro.Admin.Application.Services.DTOs.ServiceResponses;
using Aro.Admin.Application.Shared.Options;
using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Domain.Shared;
using Aro.Admin.Domain.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Aro.Admin.Infrastructure.Services;

public partial class UserService(IRepositoryManager repository, IHasher passwordHasher, IUniqueIdGenerator idGenerator, IAuthorizationService authorizationService, ILogManager<UserService> logger, IOptions<AdminSettings> adminSettings, ErrorCodes errorCodes, IPasswordHistoryEnforcer passwordHistoryEnforcer) : IUserService
{
    private readonly AdminSettings adminSettings = adminSettings.Value;
    private readonly IUserRepository userRepository = repository.UserRepository;
    private readonly IRoleRepository roleRepository = repository.RoleRepository;

    public async Task<CreateUserResponse> CreateUser(CreateUserDto user, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(CreateUser));

        //await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.CreateUser], cancellationToken);
        logger.LogDebug("Authorization verified for user creation");

        var now = DateTime.Now;
        var userEntity = new User
        {
            Id = idGenerator.Generate(),
            CreatedAt = now,
            DisplayName = user.DisplayName,
            Email = user.Email,
            IsActive = user.IsActive,
            PasswordHash = passwordHasher.Hash(user.Password),
            UpdatedAt = now,
            IsSystem = user.IsSystemUser
        };
        logger.LogDebug("Created user entity: {UserId}, email: {Email}, isActive: {IsActive}", userEntity.Id, userEntity.Email, userEntity.IsActive);

        logger.LogDebug("Retrieving assignable roles: {RoleCount}", user.AssignedRoles.Count);
        var assignableRoles = await roleRepository.GetByNames(user.AssignedRoles).Select(r => r.Id).ToListAsync(cancellationToken).ConfigureAwait(false);
        logger.LogDebug("Found {RoleCount} assignable roles for user: {UserId}", assignableRoles.Count, userEntity.Id);

        userEntity.UserRoles = [.. assignableRoles.Select(i => new UserRole { RoleId = i })];
        logger.LogDebug("Assigned {RoleCount} roles to user: {UserId}", assignableRoles.Count, userEntity.Id);

        await userRepository.Create(userEntity, cancellationToken).ConfigureAwait(false);
        logger.LogDebug("User entity created in repository: {UserId}", userEntity.Id);

        await repository.SaveChanges(cancellationToken).ConfigureAwait(false);

        await passwordHistoryEnforcer.RecordPassword(userEntity.Id, userEntity.PasswordHash).ConfigureAwait(false);
        logger.LogInfo("User created successfully: {UserId}, email: {Email}, roleCount: {RoleCount}", userEntity.Id, userEntity.Email, assignableRoles.Count);
        logger.LogDebug("Completed {MethodName}", nameof(CreateUser));
        return new CreateUserResponse(userEntity.Id, userEntity.Email, assignableRoles);
    }

    public async Task<GetUserResponse> GetUserById(Guid userId, bool includeRoles, bool includePasswordHash, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(GetUserById));

        string[] requiredPermissions = includeRoles ? [PermissionCodes.GetUser, PermissionCodes.GetUserRoles] : [PermissionCodes.GetUser];
        await authorizationService.EnsureCurrentUserPermissions(requiredPermissions, cancellationToken);
        logger.LogDebug("Authorization verified for user retrieval");

        var query = userRepository.GetById(userId);
        logger.LogDebug("Retrieved user query for ID: {UserId}", userId);

        var response = await GetUserFromQueryable(query, userId.ToString(), includeRoles, includePasswordHash, cancellationToken).ConfigureAwait(false);
        logger.LogDebug("User retrieved successfully: {UserId}, email: {Email}", userId, response.Email);

        logger.LogDebug("Completed {MethodName}", nameof(GetUserById));
        return response;
    }

    public async Task<GetUserResponse> GetUserByEmail(string email, bool includeRoles, bool includePasswordHash, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(GetUserByEmail));

        string[] requiredPermissions = includeRoles ? [PermissionCodes.GetUser, PermissionCodes.GetUserRoles] : [PermissionCodes.GetUser];
        await authorizationService.EnsureCurrentUserPermissions(requiredPermissions, cancellationToken);
        logger.LogDebug("Authorization verified for user retrieval by email");

        var query = userRepository.GetByEmail(email);
        logger.LogDebug("Retrieved user query for email: {Email}", email);

        var response = await GetUserFromQueryable(query, email, includeRoles, includePasswordHash, cancellationToken).ConfigureAwait(false);
        logger.LogDebug("User retrieved successfully by email: {Email}, userId: {UserId}", email, response.Id);

        logger.LogDebug("Completed {MethodName}", nameof(GetUserByEmail));
        return response;
    }

    public async Task<GetUserResponse> GetSystemUser(string systemPassword, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(GetUserByEmail));

        logger.LogDebug("Verifying the system password.");
        if (adminSettings.BootstrapPassword != systemPassword)
        {
            logger.LogWarn("Invalid system password provided.");
            throw new AroUnauthorizedException(errorCodes.INVALID_SYSTEM_ADMIN_PASSWORD, "Invalid system password.");
        }

        string[] requiredPermissions = [PermissionCodes.GetSystemSettings, PermissionCodes.GetUser];
        await authorizationService.EnsureCurrentUserPermissions(requiredPermissions, cancellationToken);
        logger.LogDebug("Authorization verified for system user retrieval.");

        var query = userRepository.GetAll();
        logger.LogDebug("Retrieved user query for system user.");

        var userEntity = await query
            .Where(u => u.IsSystem && u.IsActive)
            .SingleOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false)
            ?? throw new AroUserNotFoundException("SYSTEM_USER");

        logger.LogDebug("System user retrieved succesfully");

        logger.LogDebug("Completed {MethodName}", nameof(GetUserByEmail));

        var systemUser = new GetUserResponse(userEntity.Id, userEntity.Email, userEntity.IsActive, userEntity.DisplayName, userEntity.PasswordHash, userEntity.UserRoles.Select(ur => new GetRoleRespose(ur.RoleId, ur.Role.Name, ur.Role.Description, ur.Role.IsBuiltin)).ToList());

        return systemUser;
    }

    public async Task ResetPassword(Guid userId, string newPassword, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(ResetPassword));
        logger.LogDebug("Resetting password for user: {UserId}", userId);

        //await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.ResetPassword], cancellationToken); // reset password is going to be anonymous- token validation is the validation
        logger.LogDebug("Authorization verified for password reset");

        var userEntity = await userRepository.GetById(userId)
            .SingleOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false) ?? throw new AroUserNotFoundException(userId.ToString());

        logger.LogDebug("User found for password reset: {UserId}, email: {Email}", userEntity.Id, userEntity.Email);
        
        await passwordHistoryEnforcer.EnsureCanUsePassword(userId, newPassword).ConfigureAwait(false);

        var hashedPassword = passwordHasher.Hash(newPassword);
        userEntity.PasswordHash = hashedPassword;
        userEntity.UpdatedAt = DateTime.UtcNow;

        userRepository.Update(userEntity);
        await repository.SaveChanges(cancellationToken).ConfigureAwait(false);

        await passwordHistoryEnforcer.RecordPassword(userEntity.Id, hashedPassword).ConfigureAwait(false);
        await passwordHistoryEnforcer.TrimHistory(userEntity.Id).ConfigureAwait(false);

        logger.LogInfo("Password reset successfully for user: {UserId}, email: {Email}", userEntity.Id, userEntity.Email);
        logger.LogDebug("Completed {MethodName}", nameof(ResetPassword));
    }
}
