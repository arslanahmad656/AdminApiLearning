using Aro.Booking.Application.Mediator.Room.Commands;
using Aro.Booking.Application.Services.Room;
using MediatR;
using MediatorDTOs = Aro.Booking.Application.Mediator.Room.DTOs;
using ServiceDTOs = Aro.Booking.Application.Services.Room;

namespace Aro.Booking.Application.Mediator.Room.Handlers;

public class ReorderRoomsCommandHandler(IRoomService roomService) : IRequestHandler<ReorderRoomsCommand, MediatorDTOs.ReorderRoomsResponse>
{
    public async Task<MediatorDTOs.ReorderRoomsResponse> Handle(ReorderRoomsCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        var roomOrders = req.RoomOrders.Select(ro => new ServiceDTOs.ReorderRoomsDto.RoomOrderItem(ro.RoomId, ro.DisplayOrder)).ToList();

        var res = await roomService.ReorderRooms(
            new ServiceDTOs.ReorderRoomsDto(
                req.PropertyId,
                roomOrders
            ), cancellationToken
        ).ConfigureAwait(false);

        return new MediatorDTOs.ReorderRoomsResponse(res.Success, res.UpdatedCount);
    }
}
