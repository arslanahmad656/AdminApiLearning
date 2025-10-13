using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceParameters;
using Aro.Admin.Application.Services.DTOs.ServiceResponses;
using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Domain.Shared;
using Aro.Admin.Domain.Shared.Exceptions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Aro.Admin.Infrastructure.Services;

public partial class UserService(IRepositoryManager repository, IHasher passwordHasher, IUniqueIdGenerator idGenerator, IAuthorizationService authorizationService, IMapper mapper, ILogManager<UserService> logger) : IUserService
{
    private readonly IUserRepository userRepository = repository.UserRepository;
    private readonly IRoleRepository roleRepository = repository.RoleRepository;

    public async Task<CreateUserResponse> CreateUser(CreateUserDto user, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(CreateUser));
        
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.CreateUser], cancellationToken);
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
        logger.LogInfo("User created successfully: {UserId}, email: {Email}, roleCount: {RoleCount}", userEntity.Id, userEntity.Email, assignableRoles.Count);

        logger.LogDebug("Completed {MethodName}", nameof(CreateUser));
        return new(userEntity.Id, userEntity.Email, assignableRoles);
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
}
