using Aro.Booking.Application.Mediator.Room.Commands;
using Aro.Booking.Application.Mediator.Room.Notifications;
using Aro.Booking.Application.Services.Room;
using MediatR;

namespace Aro.Booking.Application.Mediator.Room.Handlers;

public class ActivateRoomCommandHandler(IRoomService roomService, IMediator mediator) : IRequestHandler<ActivateRoomCommand, DTOs.ActivateRoomResponse>
{
    public async Task<DTOs.ActivateRoomResponse> Handle(ActivateRoomCommand request, CancellationToken cancellationToken)
    {
        var req = request.ActivateRoomRequest;
        var res = await roomService.ActivateRoom(
            new(
                req.Id
            ), cancellationToken
        ).ConfigureAwait(false);

        var result = new DTOs.ActivateRoomResponse(
            res.Id,
            res.IsActive
        );

        await mediator.Publish(new RoomActivatedNotification(result), cancellationToken).ConfigureAwait(false);

        return result;
    }
}

