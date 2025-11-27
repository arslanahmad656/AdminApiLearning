using Aro.Common.Domain.Shared;
using Aro.Common.Domain.Shared.Exceptions;

namespace Aro.Booking.Domain.Shared.Exceptions;

public class PropertyNotFoundException : AroNotFoundException
{
    public PropertyNotFoundException(string propertyIdentifier, Exception? innerException = null)
        : base(
            new ErrorCodes().PROPERTY_NOT_FOUND,
            new EntityTypes().Property,
            propertyIdentifier,
            innerException)
    {
    }
}
