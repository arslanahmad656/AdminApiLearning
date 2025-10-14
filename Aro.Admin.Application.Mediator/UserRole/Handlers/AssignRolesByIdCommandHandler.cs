using Aro.Admin.Application.Mediator.UserRole.Commands;
using Aro.Admin.Application.Mediator.UserRole.DTOs;
using Aro.Admin.Application.Mediator.UserRole.Notifications;
using Aro.Admin.Application.Services.DataServices;
using MediatR;

namespace Aro.Admin.Application.Mediator.UserRole.Handlers;

public class AssignRolesByIdCommandHandler(IRoleService roleService, IMediator mediator) : IRequestHandler<AssignRolesByIdCommand, AssignRolesByIdResponse>
{
    public async Task<AssignRolesByIdResponse> Handle(AssignRolesByIdCommand request, CancellationToken cancellationToken)
    {
        var userIds = request.Data.UserIds.ToList();
        var roleIds = request.Data.RoleIds.ToList();

        await roleService.AssignRolesToUsers(userIds, roleIds, cancellationToken).ConfigureAwait(false);

        await mediator.Publish(new AssignRolesByIdNotification(new AssignRolesByIdResponse(userIds, roleIds)), cancellationToken).ConfigureAwait(false);

        return new AssignRolesByIdResponse(userIds, roleIds);
    }
}
