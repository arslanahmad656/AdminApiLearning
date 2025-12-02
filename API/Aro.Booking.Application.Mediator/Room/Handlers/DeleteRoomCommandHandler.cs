using Aro.Booking.Application.Mediator.Room.Commands;
using Aro.Booking.Application.Mediator.Room.Notifications;
using Aro.Booking.Application.Services.Room;
using MediatR;

namespace Aro.Booking.Application.Mediator.Room.Handlers;

public class DeleteRoomCommandHandler(IRoomService roomService, IMediator mediator) : IRequestHandler<DeleteRoomCommand, DTOs.DeleteRoomResponse>
{
    public async Task<DTOs.DeleteRoomResponse> Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
    {
        var req = request.DeleteRoomRequest;
        var res = await roomService.DeleteRoom(
            new(
                req.Id
            ), cancellationToken
        ).ConfigureAwait(false);

        var result = new DTOs.DeleteRoomResponse(
                res.Id
            );
        await mediator.Publish(new RoomDeletedNotification(result), cancellationToken).ConfigureAwait(false);

        return result;
    }
}
