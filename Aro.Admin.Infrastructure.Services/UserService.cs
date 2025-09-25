using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceParameters;
using Aro.Admin.Application.Services.DTOs.ServiceResponses;
using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace Aro.Admin.Infrastructure.Services;

public class UserService(IRepositoryManager repository, IPasswordHasher passwordHasher, IEntityIdGenerator idGenerator, IRoleService roleService) : IUserService
{
    private readonly IUserRepository userRepository = repository.UserRepository;
    private readonly IRoleRepository roleRepository = repository.RoleRepository;

    public async Task<CreateUserResponse> CreateUser(CreateUserDto user, CancellationToken cancellationToken = default)
    {
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
}
