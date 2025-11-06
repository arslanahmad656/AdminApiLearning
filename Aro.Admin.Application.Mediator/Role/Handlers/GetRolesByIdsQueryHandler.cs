using Aro.Admin.Application.Mediator.Role.Queries;
using Aro.Admin.Application.Mediator.Shared.DTOs;
using Aro.Admin.Application.Services.Role;
using MediatR;

namespace Aro.Admin.Application.Mediator.Role.Handlers;

public class GetRolesByIdsQueryHandler(IRoleService roleService) : IRequestHandler<GetRolesByIdsQuery, List<GetRoleResponse>>
{
    public async Task<List<GetRoleResponse>> Handle(GetRolesByIdsQuery request, CancellationToken cancellationToken)
    {
        var roles = await roleService.GetByIds(request.Data.RoleIds, cancellationToken).ConfigureAwait(false);

        return roles.Select(r => new GetRoleResponse(r.Id, r.Name, r.Description, r.IsBuiltin)).ToList();
    }
}
