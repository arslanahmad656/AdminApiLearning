using Aro.Admin.Application.Mediator.UserRole.Commands;
using Aro.Admin.Application.Mediator.UserRole.DTOs;
using Aro.Admin.Application.Mediator.UserRole.Notifications;
using Aro.Admin.Application.Services.DataServices;
using MediatR;

namespace Aro.Admin.Application.Mediator.UserRole.Handlers;

public class RevokeRolesByIdCommandHandler(IRoleService roleService, IMediator mediator) : IRequestHandler<RevokeRolesByIdCommand, RevokeRolesByIdResponse>
{
    public async Task<RevokeRolesByIdResponse> Handle(RevokeRolesByIdCommand request, CancellationToken cancellationToken)
    {
        var userIds = request.Data.UserIds.ToList();
        var roleIds = request.Data.RoleIds.ToList();

        await roleService.RevokeRolesFromUsers(userIds, roleIds, cancellationToken).ConfigureAwait(false);

        await mediator.Publish(new RevokeRolesByIdNotification(new RevokeRolesByIdResponse(userIds, roleIds)), cancellationToken).ConfigureAwait(false);

        return new RevokeRolesByIdResponse(userIds, roleIds);
    }
}
