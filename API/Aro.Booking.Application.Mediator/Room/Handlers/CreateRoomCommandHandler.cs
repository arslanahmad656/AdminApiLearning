using Aro.Booking.Application.Mediator.Room.Commands;
using Aro.Booking.Application.Mediator.Room.Notifications;
using Aro.Booking.Application.Services.Room;
using MediatR;

namespace Aro.Booking.Application.Mediator.Room.Handlers;

public class CreateRoomCommandHandler(IRoomService roomService, IMediator mediator) : IRequestHandler<CreateRoomCommand, DTOs.CreateRoomResponse>
{
    public async Task<DTOs.CreateRoomResponse> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        var r = request.CreateRoomRequest;
        var response = await roomService.CreateRoom(
            new CreateRoomDto(
                r.PropertyId,
                r.RoomName,
                r.RoomCode,
                r.Description,
                r.MaxOccupancy,
                r.MaxAdults,
                r.MaxChildren,
                r.RoomSizeSQM,
                (BedConfiguration)r.BedConfig,
                r.Amenities,
                r.RoomImages?
                    .Select(img => new RoomImage(
                        img.Name,
                        img.Content,
                        img.OrderIndex,
                        img.IsThumbnail))
                    .ToList()
                    ?? [],
                r.IsActive
            ), cancellationToken
        ).ConfigureAwait(false);

        var result = new DTOs.CreateRoomResponse(response.Id, response.RoomName);
        await mediator.Publish(new RoomCreatedNotification(result), cancellationToken).ConfigureAwait(false);

        return result;
    }
}
