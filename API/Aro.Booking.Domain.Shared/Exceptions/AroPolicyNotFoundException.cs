using Aro.Common.Domain.Shared;
using Aro.Common.Domain.Shared.Exceptions;

namespace Aro.Booking.Domain.Shared.Exceptions;

public class AroPolicyNotFoundException(string policyIdentifier, Exception? innerException)
    : AroNotFoundException(new ErrorCodes().POLICY_NOT_FOUND, new EntityTypes().Policy, policyIdentifier, innerException)
{
    public AroPolicyNotFoundException(string policyIdentifier) : this(policyIdentifier, null)
    {
    }
}

