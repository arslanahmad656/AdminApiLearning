using Aro.Booking.Application.Mediator.Room.Queries;
using Aro.Booking.Application.Services.Room;
using MediatR;

namespace Aro.Booking.Application.Mediator.Room.Handlers;

public class GetRoomsCommandHandler(IRoomService roomService) : IRequestHandler<GetRoomsQuery, DTOs.GetRoomsResponse>
{
    public async Task<DTOs.GetRoomsResponse> Handle(GetRoomsQuery request, CancellationToken cancellationToken)
    {
        var req = request.Data;
        var res = await roomService.GetRooms(
            new(
                req.PropertyId,
                req.Filter,
                req.Include ?? string.Empty,
                req.Page,
                req.PageSize,
                req.SortBy,
                req.Ascending
                ), cancellationToken).ConfigureAwait(false);

        var roomDtos = res.Rooms?
            .Select(r => new DTOs.RoomDto(
                r.Id,
                r.RoomName,
                r.RoomCode,
                r.Description,
                r.MaxOccupancy,
                r.MaxAdults,
                r.MaxChildren,
                r.RoomSizeSQM,
                (DTOs.BedConfiguration)r.BedConfig,
                r.AmenityIds,
                r.IsActive,
                r.Images?.Select(img => new DTOs.RoomImageInfoDto(
                    img.FileId,
                    img.OrderIndex,
                    img.IsThumbnail
                )).ToList()
            ))
            .ToList() ?? [];

        var result = new DTOs.GetRoomsResponse(roomDtos, res.TotalCount);

        return result;
    }
}
