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

public partial class UserService(IRepositoryManager repository, IPasswordHasher passwordHasher, IEntityIdGenerator idGenerator, IAuthorizationService authorizationService, PermissionCodes permissionCodes, IMapper mapper) : IUserService
{
    private readonly IUserRepository userRepository = repository.UserRepository;
    private readonly IRoleRepository roleRepository = repository.RoleRepository;

    public async Task<CreateUserResponse> CreateUser(CreateUserDto user, CancellationToken cancellationToken = default)
    {
        await authorizationService.EnsureCurrentUserPermissions([permissionCodes.CreateUser], cancellationToken);

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

        var assignableRoles = await roleRepository.GetByNames(user.AssignedRoles).Select(r => r.Id).ToListAsync(cancellationToken).ConfigureAwait(false);
        userEntity.UserRoles = [.. assignableRoles.Select(i => new UserRole { RoleId = i })];

        await userRepository.Create(userEntity, cancellationToken).ConfigureAwait(false);
        await repository.SaveChanges(cancellationToken).ConfigureAwait(false);

        return new(userEntity.Id, userEntity.Email, assignableRoles); 
    }

    public async Task<GetUserResponse> GetUserById(Guid userId, bool includeRoles, bool includePasswordHash, CancellationToken cancellationToken = default)
    {
        string[] requiredPermissions = includeRoles ? [permissionCodes.GetUser, permissionCodes.GetUserRoles] : [permissionCodes.GetUser];
        await authorizationService.EnsureCurrentUserPermissions(requiredPermissions, cancellationToken);

        var query = userRepository.GetById(userId);
        var response = await GetUserFromQueryable(query, userId.ToString(), includeRoles, includePasswordHash, cancellationToken).ConfigureAwait(false);

        return response;
    }

    public async Task<GetUserResponse> GetUserByEmail(string email, bool includeRoles, bool includePasswordHash, CancellationToken cancellationToken = default)
    {
        string[] requiredPermissions = includeRoles ? [permissionCodes.GetUser, permissionCodes.GetUserRoles] : [permissionCodes.GetUser];
        await authorizationService.EnsureCurrentUserPermissions(requiredPermissions, cancellationToken);

        var query = userRepository.GetByEmail(email);
        var response = await GetUserFromQueryable(query, email, includeRoles, includePasswordHash, cancellationToken).ConfigureAwait(false);

        return response;
    }
}
