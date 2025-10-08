namespace Aro.Admin.Domain.Shared.Exceptions;

public class UserNotFoundException(string userIdenifier, Exception? innerException) : NotFoundException(new ErrorCodes().USER_NOT_FOUND, new EntityTypes().User, userIdenifier.ToString(), innerException)
{
    public UserNotFoundException(string userIdenifier) : this(userIdenifier, null)
    {
        
    }
}
