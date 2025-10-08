using Aro.Admin.Application.Mediator.UserRole.Queries;
using Aro.Admin.Application.Services.DataServices;
using MediatR;

namespace Aro.Admin.Application.Mediator.UserRole.Handlers;

public class UserHasRoleQueryHandler(IRoleService roleService) : IRequestHandler<UserHasRoleQuery, bool>
{
    public async Task<bool> Handle(UserHasRoleQuery request, CancellationToken cancellationToken)
    {
        var hasRole = await roleService.UserHasRole(request.Data.UserId, request.Data.RoleId, cancellationToken).ConfigureAwait(false);

        return hasRole;
    }
}
