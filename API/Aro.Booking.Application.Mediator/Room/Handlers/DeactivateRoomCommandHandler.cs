using Aro.Booking.Application.Mediator.Room.Commands;
using Aro.Booking.Application.Mediator.Room.Notifications;
using Aro.Booking.Application.Services.Room;
using MediatR;

namespace Aro.Booking.Application.Mediator.Room.Handlers;

public class DeactivateRoomCommandHandler(IRoomService roomService, IMediator mediator) : IRequestHandler<DeactivateRoomCommand, DTOs.DeactivateRoomResponse>
{
    public async Task<DTOs.DeactivateRoomResponse> Handle(DeactivateRoomCommand request, CancellationToken cancellationToken)
    {
        var req = request.DeactivateRoomRequest;
        var res = await roomService.DeactivateRoom(
            new(
                req.Id
            ), cancellationToken
        ).ConfigureAwait(false);

        var result = new DTOs.DeactivateRoomResponse(
            res.Id,
            res.IsActive
        );

        await mediator.Publish(new RoomDeactivatedNotification(result), cancellationToken).ConfigureAwait(false);

        return result;
    }
}

