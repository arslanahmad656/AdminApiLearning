using Aro.Booking.Application.Mediator.Room.Queries;
using Aro.Booking.Application.Services.Room;
using MediatR;

namespace Aro.Booking.Application.Mediator.Room.Handlers;

public class GetRoomCommandHandler(IRoomService roomService) : IRequestHandler<GetRoomQuery, DTOs.GetRoomResponse>
{
    public async Task<DTOs.GetRoomResponse> Handle(GetRoomQuery request, CancellationToken cancellationToken)
    {
        var req = request.Data;
        var res = await roomService.GetRoomById(
            new(
                req.Id,
                req.Include ?? string.Empty
                ), cancellationToken).ConfigureAwait(false);

        var r = res.Room;
        var roomDto = new DTOs.RoomDto(
            r.Id,
            r.RoomName,
            r.RoomCode,
            r.Description,
            r.MaxOccupancy,
            r.MaxAdults,
            r.MaxChildren,
            r.RoomSizeSQM,
            r.RoomSizeSQFT,
            (DTOs.BedConfiguration)r.BedConfig,
            r.AmenityIds,
            r.IsActive
            );

        var result = new DTOs.GetRoomResponse(roomDto);

        return result;
    }
}
