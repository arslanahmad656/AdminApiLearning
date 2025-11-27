using Aro.Common.Domain.Shared;
using Aro.Common.Domain.Shared.Exceptions;

namespace Aro.Booking.Domain.Shared.Exceptions;

public class PropertyAlreadyExistsException : AroException
{
    public PropertyAlreadyExistsException(string propertyName, string? groupId, Exception? innerException = null)
        : base(
            new ErrorCodes().PROPERTY_ALREADY_EXISTS,
            $"Property with name '{propertyName}' already exists for group '{groupId}'.",
            innerException)
    {
    }
}
