using Aro.Booking.Application.Mediator.Room.Queries;
using Aro.Booking.Application.Services.Room;
using MediatR;

namespace Aro.Booking.Application.Mediator.Room.Handlers;

public class RoomCodeExistsQueryHandler(IRoomService roomService) : IRequestHandler<RoomCodeExistsQuery, bool>
{
    public async Task<bool> Handle(RoomCodeExistsQuery request, CancellationToken cancellationToken)
    {
        var req = request.Data;
        var res = await roomService.RoomCodeExists(new(req.PropertyId, req.RoomCode), cancellationToken).ConfigureAwait(false);

        return res;
    }
}
