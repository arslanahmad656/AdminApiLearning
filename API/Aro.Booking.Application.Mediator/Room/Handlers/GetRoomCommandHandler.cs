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

        // Map images if present
        List<DTOs.RoomImageInfoDto>? images = null;
        if (r.Images != null)
        {
            images = r.Images.Select(img => new DTOs.RoomImageInfoDto(
                img.FileId,
                img.OrderIndex,
                img.IsThumbnail
            )).ToList();
        }

        var roomDto = new DTOs.RoomDto(
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
            images
            );

        var result = new DTOs.GetRoomResponse(roomDto);

        return result;
    }
}
