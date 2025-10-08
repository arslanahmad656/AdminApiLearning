using Aro.Admin.Application.Mediator.Shared.DTOs;
using Aro.Admin.Application.Mediator.UserRole.Queries;
using Aro.Admin.Application.Services.DataServices;
using AutoMapper;
using MediatR;

namespace Aro.Admin.Application.Mediator.UserRole.Handlers;

public class GetUserRolesQueryHandler(IRoleService roleService, IMapper mapper) : IRequestHandler<GetUserRolesQuery, List<GetRoleResponse>>
{
    public async Task<List<GetRoleResponse>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await roleService.GetUserRoles(request.Data.UserId, cancellationToken).ConfigureAwait(false);

        return mapper.Map<List<GetRoleResponse>>(roles);
    }
}
