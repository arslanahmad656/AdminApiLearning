using Aro.Common.Domain.Shared;
using Aro.Common.Domain.Shared.Exceptions;

namespace Aro.Booking.Domain.Shared.Exceptions;

public class AroRoomNotFoundException(string roomIdenifier, Exception? innerException) : AroNotFoundException(new ErrorCodes().ROOM_NOT_FOUND, new EntityTypes().Room, roomIdenifier.ToString(), innerException)
{
    public AroRoomNotFoundException(string roomIdenifier) : this(roomIdenifier, null)
    {

    }
}
