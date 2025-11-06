using Aro.Common.Domain.Shared;
using Aro.Common.Domain.Shared.Exceptions;

namespace Aro.Admin.Domain.Shared.Exceptions;

public class AroUserNotFoundException(string userIdenifier, Exception? innerException) : AroNotFoundException(new ErrorCodes().USER_NOT_FOUND, new EntityTypes().User, userIdenifier.ToString(), innerException)
{
    public AroUserNotFoundException(string userIdenifier) : this(userIdenifier, null)
    {

    }
}
