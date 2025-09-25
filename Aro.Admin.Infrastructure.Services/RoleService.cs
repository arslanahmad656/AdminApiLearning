using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceResponses;
using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Repository;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Aro.Admin.Infrastructure.Services;

public class RoleService(IRepositoryManager repository, IMapper mapper) : IRoleService
{
    private readonly IUserRoleRepository userRoleRepository = repository.UserRoleRepository;
    private readonly IRoleRepository roleRepository = repository.RoleRepository;

    public async Task AssignRolesToUsers(IEnumerable<Guid> userIds, IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default)
    {
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
        var rolesToAdd = await GetByNames(roleNames, cancellationToken).ConfigureAwait(false);
        var roleIds = rolesToAdd.Select(r => r.Id);

        await AssignRolesToUsers(userIds, roleIds, cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<GetRoleRespose>> GetByNames(IEnumerable<string> names, CancellationToken cancellationToken = default)
    {
        var roleEntities = await roleRepository
            .GetByNames(names)
            .ToListAsync(cancellationToken);

        var roles = mapper.Map<List<GetRoleRespose>>(roleEntities);

        return roles;
    }
}
