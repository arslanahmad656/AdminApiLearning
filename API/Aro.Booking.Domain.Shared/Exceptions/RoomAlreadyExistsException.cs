using Aro.Common.Domain.Shared;
using Aro.Common.Domain.Shared.Exceptions;

namespace Aro.Booking.Domain.Shared.Exceptions;

public class RoomAlreadyExistsException(string roomCode, string? roomId, Exception? innerException = null) : AroException(
        new ErrorCodes().ROOM_ALREADY_EXISTS,
        $"Room with code '{roomCode}' already exists for group '{roomId}'.",
        innerException)
{
}
