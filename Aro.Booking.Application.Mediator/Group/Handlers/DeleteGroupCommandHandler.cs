using Aro.Booking.Application.Mediator.Group.Commands;
using Aro.Booking.Application.Mediator.Group.Notifications;
using Aro.Booking.Application.Services.Group;
using MediatR;
using DeleteGroupResponse = Aro.Booking.Application.Mediator.Group.DTOs.DeleteGroupResponse;

namespace Aro.Booking.Application.Mediator.Group.Handlers;

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
