using Aro.Common.Domain.Shared;
using Aro.Common.Domain.Shared.Exceptions;

namespace Aro.Booking.Domain.Shared.Exceptions;

public class AroAmenityNotFoundException(string amenityIdenifier, Exception? innerException) : AroNotFoundException(new ErrorCodes().AMENITY_NOT_FOUND, new EntityTypes().Amenity, amenityIdenifier.ToString(), innerException)
{
    public AroAmenityNotFoundException(string amenityIdenifier) : this(amenityIdenifier, null)
    {

    }
}
