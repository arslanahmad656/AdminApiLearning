using Aro.Admin.Application.Mediator.Role.Queries;
using Aro.Admin.Application.Mediator.Shared.DTOs;
using Aro.Admin.Application.Services.DataServices;
using AutoMapper;
using MediatR;

namespace Aro.Admin.Application.Mediator.Role.Handlers;

public class GetRolesByIdsQueryHandler(IRoleService roleService, IMapper mapper) : IRequestHandler<GetRolesByIdsQuery, List<GetRoleResponse>>
{
    public async Task<List<GetRoleResponse>> Handle(GetRolesByIdsQuery request, CancellationToken cancellationToken)
    {
        var roles = await roleService.GetByIds(request.Data.RoleIds, cancellationToken).ConfigureAwait(false);

        return mapper.Map<List<GetRoleResponse>>(roles);
    }
}
