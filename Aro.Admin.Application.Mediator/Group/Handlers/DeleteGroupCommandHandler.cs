using Aro.Admin.Application.Mediator.Group.Commands;
using Aro.Admin.Application.Mediator.Group.DTOs;
using Aro.Admin.Application.Mediator.Group.Notifications;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceParameters;
using MediatR;

namespace Aro.Admin.Application.Mediator.Group.Handlers;

public class DeleteGroupCommandHandler(IGroupService groupService, IMediator mediator) : IRequestHandler<DeleteGroupCommand, DeleteGroupResponse>
{
    public async Task<DeleteGroupResponse> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        var req = request.DeleteGroupRequest;
        var res = await groupService.DeleteGroup(
            new(
                req.Id
            ), cancellationToken
        ).ConfigureAwait(false);

        var result = new DeleteGroupResponse(
                res.Id
            );
        await mediator.Publish(new GroupDeletedNotification(result), cancellationToken).ConfigureAwait(false);

        return result;
    }
}
