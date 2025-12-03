using Aro.Booking.Application.Mediator.Room.Commands;
using Aro.Booking.Application.Mediator.Room.Notifications;
using Aro.Booking.Application.Services.Room;
using MediatR;

namespace Aro.Booking.Application.Mediator.Room.Handlers;

public class PatchRoomCommandHandler(IRoomService roomService, IMediator mediator) : IRequestHandler<PatchRoomCommand, DTOs.PatchRoomResponse>
{
    public async Task<DTOs.PatchRoomResponse> Handle(PatchRoomCommand request, CancellationToken cancellationToken)
    {
        var req = request.PatchRoomRequest.Room;
        var res = await roomService.PatchRoom(new(new(
                req.Id,
                req.RoomName,
                req.RoomCode,
                req.Description,
                req.MaxOccupancy,
                req.MaxAdults,
                req.MaxChildren,
                req.RoomSizeSQM,
                req.RoomSizeSQFT,
                (BedConfiguration?)req.BedConfig,
                req.AmenityIds,
                req.IsActive
            )), cancellationToken
        ).ConfigureAwait(false);

        var r = res.Room;
        var result = new DTOs.PatchRoomResponse(new(
                r.Id,
                r.RoomName,
                r.RoomCode,
                r.Description,
                r.MaxOccupancy,
                r.MaxAdults,
                r.MaxChildren,
                r.RoomSizeSQM,
                r.RoomSizeSQFT,
                (DTOs.BedConfiguration?)r.BedConfig,
                r.AmenityIds,
                r.IsActive
            ));

        await mediator.Publish(new RoomPatchedNotification(result), cancellationToken).ConfigureAwait(false);

        return result;
    }
}
